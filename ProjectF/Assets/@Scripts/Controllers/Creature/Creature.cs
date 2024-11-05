using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static Define;

public class Creature : BaseObject
{
  public BaseObject Target { get; set; }
  public List<BaseObject> SupplyTargets { get; set; } = new List<BaseObject>();
  public List<BaseObject> SupplyStorage { get; set; } = new List<BaseObject>(); 
  public List<Data.SkillData> Skills { get; protected set; } = new List<Data.SkillData>();
  public float Speed { get; protected set; } = 1.0f;
  public FCreatureType CreatureType { get; protected set; } = FCreatureType.None;

  public Dictionary<FJob, float> JobDic = new Dictionary<FJob, float>();
  public Dictionary<FPersonalJob, float> PersonalDic = new Dictionary<FPersonalJob, float>();

  public KeyValuePair<Enum, float> CurrentJob => new KeyValuePair<Enum, float>(job, GetJobPriority(job));

  public event Action<KeyValuePair<FJob, float>> jobChanged;
  private KeyValuePair<FJob, float> jobChangedPair;
  public KeyValuePair<FJob, float> JobChanged
  {
    get { return jobChangedPair; }
    set
    {
      jobChangedPair = value;
      jobChanged?.Invoke(jobChangedPair);
    }
  }

  public event Action<KeyValuePair<FPersonalJob, float>> pjobChanged;
  private KeyValuePair<FPersonalJob, float> pjobChangedPair;
  public KeyValuePair<FPersonalJob, float> PJobChanged
  {
    get { return pjobChangedPair; }
    set
    {
      pjobChangedPair = value;
      pjobChanged?.Invoke(pjobChangedPair);
    }
  }

  float DistToTargetSqr(BaseObject target = null)
  {
    BaseObject _target = null;
    if (target == null) _target = Target;
    else _target = target;

    Vector3 dir = (_target.transform.position - transform.position);
    float distToTarget = Math.Max(0, dir.magnitude - _target.ExtraCells * 1f - ExtraCells * 1f); // TEMP
    return distToTarget * distToTarget;
  }

  public Data.CreatureData CreatureData { get; protected set; }
  public JobSystem jobSystem;
  public PersonalPrioritySystem ppSystem;
  protected float oneMoveMagnititue;

  
  public FJobPhase jobPhase = FJobPhase.None;
  protected BaseObject supplyTarget;

  #region Stats
  [SerializeField] private float _maxHp;
  [SerializeField] private float _Atk;
  [SerializeField] private float _Calories = 10000f;
  [SerializeField] private float _SupplyCapacity;
  [SerializeField] private float _CurrentSupply;

  public float maxHp { get { return _maxHp; } set { _maxHp = value; } }
  public float Atk { get { return _Atk; } set { _Atk = value; } }
  public float Calories { get { return _Calories; } set { _Calories = value; } }
  public float SupplyCapacity { get { return _SupplyCapacity; } set { _SupplyCapacity = value; } }
  public float CurrentSupply { get { return _CurrentSupply; } set { _CurrentSupply = value; } }
  #endregion

  protected FCreatureState creatureState = FCreatureState.None;
  public virtual FCreatureState CreatureState
  {
    get { return creatureState; }
    set
    {
      if (creatureState != value)
      {
        creatureState = value;
        UpdateAnimation();
      }
    }
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    oneMoveMagnititue = Managers.Map.CellGrid.cellSize.x;
    ObjectType = FObjectType.Creature;
    jobSystem = this.GetComponent<JobSystem>();
    ppSystem = this.GetComponent<PersonalPrioritySystem>();

    var jobLength = Enum.GetValues(typeof(FJob)).Length;
    var personalLength = Enum.GetValues(typeof(FPersonalJob)).Length;

    for(int i = 0; i < jobLength; i++) JobDic.Add((FJob)i, 0);
    for(int i = 0; i < personalLength; i++) PersonalDic.Add((FPersonalJob)i, 0);


    //FIXME
    //StartCoroutine(CoLerpToCellPos());
    

    return true;
  }

  public void SetOrAddJobPriority(Enum job, float p, bool set = false)
  {
    if (job is FJob)
    {
      FJob tmpJob = (FJob)job;
      if (set) JobDic[tmpJob] = p;
      else
      {
        if (JobDic.TryGetValue(tmpJob, out var value))
          JobDic[tmpJob] += p;
      }
      JobChanged = new KeyValuePair<FJob, float>(tmpJob, JobDic[tmpJob]);
    }
    else if(job is FPersonalJob)
    {
      FPersonalJob tmpJob = (FPersonalJob)job;
      if (set) PersonalDic[tmpJob] = p;
      else
      {
        if (PersonalDic.TryGetValue(tmpJob, out var value))
          PersonalDic[tmpJob] += p;
      }
      PJobChanged = new KeyValuePair<FPersonalJob, float>(tmpJob, PersonalDic[tmpJob]);
    }
  }

  public float GetJobPriority(Enum job)
  {
    if(job is FJob)
    {
      if (JobDic.TryGetValue((FJob)job, out var value))
        return value;
    }
    else if(job is FPersonalJob)
    {
      if (PersonalDic.TryGetValue((FPersonalJob)job, out var value))
        return value;
    }
    return -1;
  }

  public virtual void SetInfo(int dataID)
  {
    dataTemplateID = dataID;
    CreatureData = Managers.Data.CreatureDic[dataID];

    gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";

    maxHp = CreatureData.maxHp;
    SupplyCapacity = CreatureData.supplyCap;
    //TODO

    CreatureState = FCreatureState.Idle;

    foreach(int skillID in CreatureData.SkillList)
    {
      Skills.Add(Managers.Data.SkillDic[skillID]);
    }

    StartCoroutine(CoLerpToCellPos());

  }

  protected override void UpdateAnimation()
  {
    switch(CreatureState)
    {
      case FCreatureState.Idle:
        PlayAnimation(CreatureData.Idle);
        break;
      case FCreatureState.Move:
        PlayAnimation(CreatureData.Move);
        break;
      case FCreatureState.Dead:
        PlayAnimation(CreatureData.Dead);
        break;
      case FCreatureState.Skill:
        PlayAnimation(Skills[0].AnimName);
        break;
      default: break;
    }
  }

  #region AI
  public float UpdateAITick { get; protected set; } = 0.0f;

  protected IEnumerator CoUpdateAI()
  {
    while (true)
    {
      switch (CreatureState)
      {
        case FCreatureState.Idle:
          UpdateIdle();
          break;
        case FCreatureState.Move:
          UpdateMove();
          break;
        case FCreatureState.Skill:
          UpdateSkill();
          break;
        case FCreatureState.Dead:
          UpdateDead();
          break;
      }

      if (UpdateAITick > 0)
        yield return new WaitForSeconds(UpdateAITick);
      else
        yield return null;
    }
  }

  protected IEnumerator CoUpdateState()
  {
    while(true)
    {
      UpdateState();
      UpdateMood();
      yield return new WaitForSeconds(3f);
    }
  }

  protected virtual void UpdateIdle() { }
  protected virtual void UpdateMove() { }
  protected virtual void UpdateSkill() { }
  protected virtual void UpdateDead() { }
  protected virtual void UpdateState() { }
  protected virtual void UpdateMood() { }

  public Enum SelectJob(/*Func<BaseObjenct, bool> func = null*/)
  {
    if (ppSystem.CurrentPersonalJob.Key is not FPersonalJob.None)
    {
      if (job is FPersonalJob && Target != null)
        return job;
      else return ppSystem.CurrentPersonalJob.Key;
    }

    if (job is FJob && Target != null)
      return job;

    return jobSystem.CurrentJob.Key;
  }

  #endregion

  #region Wait
  protected Coroutine _coWait;

  protected void StartWait(float seconds)
  {
    CancelWait();
    _coWait = StartCoroutine(CoWait(seconds));
  }

  IEnumerator CoWait(float seconds)
  {
    yield return new WaitForSeconds(seconds);
    _coWait = null;
  }

  protected void CancelWait()
  {
    if (_coWait != null)
      StopCoroutine(_coWait);
    _coWait = null;
  }
  #endregion

  #region Map
  // 일하는 목표 우선순위 외에도 자기 자신만의 작업큐를 들고있어야함
  // 그리고 그걸 일정 프레임? 다끝났을때? 마다 리프레쉬하면서 몇개(10개정도?)까지만 들고있어야하는. 일종의 jobQueue가 있어야함
  //public BaseObject SearchJob(float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null)
  //{
  //  BaseObject target = null;

  //  foreach(BaseObject obj in objs)
  //  {

  //  }
  //}

  public FFindPathResults FindPathAndMoveToCellPos(Vector3 destWorldPos, int maxDepth, bool forceMoveCloser = false)
  {
    Vector3Int destCellPos = Managers.Map.World2Cell(destWorldPos);
    return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
  }

  public FFindPathResults FindPathAndMoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
  {
    if (LerpCellPosCompleted == false)
      return FFindPathResults.Fail_LerpCell;

    if (CreatureState != FCreatureState.Move) return FFindPathResults.Success;

    // A*
    List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, maxDepth);
    if (path.Count < 2)
      return FFindPathResults.Fail_NoPath;

    if (forceMoveCloser)
    {
      Vector3Int diff1 = CellPos - destCellPos;
      Vector3Int diff2 = path[1] - destCellPos;
      if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
        return FFindPathResults.Fail_NoPath;
    }

    Vector3Int dirCellPos = path[1] - CellPos;
    //Vector3Int dirCellPos = destCellPos - CellPos;
    Vector3Int nextPos = CellPos + dirCellPos;

    if (Managers.Map.MoveTo(this, nextPos) == false)
      return FFindPathResults.Fail_MoveTo;

    return FFindPathResults.Success;
  }

  public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
  {
    if (LerpCellPosCompleted == false)
      return false;

    return Managers.Map.MoveTo(this, destCellPos);
  }

  protected IEnumerator CoLerpToCellPos()
  {
    while (true)
    {
      //Warrior player = this as Warrior;
      //if (player != null)
      //{
      //  float div = 5;
      //  Vector3 campPos = Managers.Object.Camp.Destination.transform.position;
      //  Vector3Int campCellPos = Managers.Map.World2Cell(campPos);
      //  float ratio = Math.Max(1, (CellPos - campCellPos).magnitude / div);

      //  LerpToCellPos(CreatureData.MoveSpeed * ratio);
      //}
      //else
      LerpToCellPos(CreatureData.Speed);

      yield return null;
    }
  }
  #endregion

  #region Map
  public BaseObject FindClosestInRange<T>(T job, float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null) where T : Enum
  { 
    BaseObject target = null;
    float bestDistanceSqr = float.MaxValue;
    float searchDistanceSqr = range * range;

    foreach (BaseObject obj in objs)
    {
      if (!obj.workableJob.Equals(job)) continue;
      Vector3 dir = obj.transform.position - transform.position;
      float distToTargetSqr = dir.sqrMagnitude;

      // 서치 범위보다 멀리 있으면 스킵.
      if (distToTargetSqr > searchDistanceSqr)
        continue;

      // 이미 더 좋은 후보를 찾았으면 스킵.
      if (distToTargetSqr > bestDistanceSqr)
        continue;

      // 추가 조건
      if (func != null && func.Invoke(obj) == false)
        continue;

      target = obj;
      bestDistanceSqr = distToTargetSqr;
    }

    return target;
  }

  public List<BaseObject> FindsClosestInRange<T>(T job, float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null) where T : Enum
  {
    List<BaseObject> target = new List<BaseObject>(); 
    Dictionary<BaseObject, float> targetdic = new Dictionary<BaseObject, float>();
    float searchDistanceSqr = range * range;

    foreach (BaseObject obj in objs)
    {
      if (!obj.workableJob.Equals(job)) continue;
      Vector3 dir = obj.transform.position - transform.position;
      float distToTargetSqr = dir.sqrMagnitude;

      // 서치 범위보다 멀리 있으면 스킵.
      if (distToTargetSqr > searchDistanceSqr)
        continue;

      // 추가 조건
      if (func != null && func.Invoke(obj) == false)
        continue;

      targetdic.Add(obj, distToTargetSqr);
    }

    targetdic = targetdic.OrderBy(pair => pair.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
    foreach(var dic in targetdic)
    {
      target.Add(dic.Key);
    }

    return target;
  }

  protected void ChaseOrAttackTarget(float chaseRange, float attackRange, BaseObject target = null)
  {
    BaseObject chaseTarget = null;
    if (target == null) chaseTarget = Target;
    else chaseTarget = target;

    float distToTargetSqr = DistToTargetSqr(chaseTarget);
    float attackDistanceSqr = attackRange * attackRange;

    if (distToTargetSqr <= attackDistanceSqr)
    {

      // 공격 범위 이내로 들어왔다면 공격.
     
      CreatureState = FCreatureState.Skill;
      //skill.DoSkill();
      return;
    }
    else
    {
      // 공격 범위 밖이라면 추적.
      FFindPathResults result = FindPathAndMoveToCellPos(chaseTarget.transform.position, 100);
      if(result == FFindPathResults.Fail_NoPath)
      {
        CreatureState = FCreatureState.Skill;
        //CreatureState = FCreatureState.Move;
      }
      // 너무 멀어지면 포기.
      //float searchDistanceSqr = chaseRange * chaseRange;
      //if (distToTargetSqr > searchDistanceSqr)
      //{
      //  Target = null;
      //  CreatureState = FCreatureState.Move;
      //}
      return;
    }
  }

  //protected void ChaseOrStoreTarget()
  //{
  //  if (jobPhase is JobPhase.On) return;

  //  if(SupplyTargets.Count <= 0) SupplyTargets = FindsClosestInRange(FJob.Supply, 10f, Managers.Object.ItemHolders, func: IsValid);
  //  if (SupplyTargets.Count <= 0) { jobPhase = JobPhase.Done; return; }
  //  if (jobPhase is JobPhase.Start) jobPhase = JobPhase.On;

  //  foreach (var target in SupplyTargets)
  //  {
  //    if (this.CurrentSupply >= this.SupplyCapacity)
  //    {
  //      jobPhase = JobPhase.Done;
  //      return;
  //    }

  //    ChaseOrAttackTarget(target, 100, 1.2f);
  //  }

  //  //jobPhase = JobPhase.Done;
  //  return;

  //}
  #endregion
}
