using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using System;
using System.Linq;
using static Define;

#region UtilClass
public class jobDicValue
{
  private float priority;
  private bool isAble; 

  public jobDicValue(float v1, bool v2)
  {
    this.priority = v1;
    this.isAble = v2;
  }

  public float Priority
  {
    get { return priority; }
    set { priority = value; }
  }

  public bool IsAble
  {
    get { return isAble; }
    set { isAble = value; }
  }
}
#endregion

public class Creature : BaseObject
{
  public BaseObject Target { get; set; }
  public BaseObject supplyTarget { get; set; }

  [SerializedDictionary("itemId", "itemMass")]
  public SerializedDictionary<int, float> ItemHaveList;

  //public List<BaseObject> SupplyStorage { get; set; } = new List<BaseObject>(); 
  public List<Data.SkillData> Skills { get; protected set; } = new List<Data.SkillData>();
  public float Speed { get; protected set; } = 1.0f;
  public FCreatureType CreatureType { get; protected set; } = FCreatureType.None;

  public Dictionary<FJob, jobDicValue> JobDic = new Dictionary<FJob, jobDicValue>();
  public Dictionary<FPersonalJob, float> PersonalDic = new Dictionary<FPersonalJob, float>();

  public KeyValuePair<Enum, float> CurrentJob => new KeyValuePair<Enum, float>(job, GetJobPriority(job));

  public event Action<KeyValuePair<FJob, jobDicValue>> jobChanged;
  private KeyValuePair<FJob, jobDicValue> jobChangedPair;
  public KeyValuePair<FJob, jobDicValue> JobChanged
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
    float distToTarget = Math.Max(0, dir.magnitude - _target.ExtraCellsX * 1f - ExtraCellsX * 1f); // TEMP
    return distToTarget * distToTarget;
  }

  public Data.CreatureData CreatureData { get; protected set; }
  public JobSystem jobSystem;
  public PersonalPrioritySystem ppSystem;
  protected float oneMoveMagnititue;

  
  public FJobPhase jobPhase = FJobPhase.None;

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

  protected FCreatureMoveState creatureMoveState = FCreatureMoveState.None;
  public virtual FCreatureMoveState CreatureMoveState
  {
    get { return creatureMoveState; }
    set
    {
      creatureMoveState = value;
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
    for(int i = 0; i < jobLength; i++) JobDic.Add((FJob)i, new jobDicValue(20, true));
    for(int i = 0; i < personalLength; i++) PersonalDic.Add((FPersonalJob)i, 0);

    Managers.Game.onJobAbleChanged -= SetJobIsAble;
    Managers.Game.onJobAbleChanged += SetJobIsAble;
    //FIXME
    //StartCoroutine(CoLerpToCellPos());
    

    return true;
  }

  
  #region Supply
  public void AddHaveList(int dataID, float mass)
  {
    if (CurrentSupply + mass > SupplyCapacity) return;

    if (ItemHaveList.ContainsKey(dataID))
    {
      ItemHaveList[dataID] += mass;
    }
    else ItemHaveList.Add(dataID, mass);

    CurrentSupply += mass;
  }

  public bool SearchHaveList(int dataID)
  {
    if (ItemHaveList.ContainsKey(dataID)
      && ItemHaveList[dataID] > 0)
    return true;
    return false;
  }

  public float SupplyFromHaveList(int dataID, float mass)
  {
    float result = 0;

    if(ItemHaveList.ContainsKey(dataID))
    {
      if(ItemHaveList[dataID] - mass < 0)
      {
        result = ItemHaveList[dataID];
        ItemHaveList.Remove(dataID);
      }
      else
      {
        ItemHaveList[dataID] -= mass;
        result = mass;
      }
    }

    if (ItemHaveList[dataID] <= 0) ItemHaveList.Remove(dataID);

    CurrentSupply -= result;

    Managers.Object.RemoveItem(dataID, mass);
    return result;
  }
  #endregion

  public void SetOrAddJobPriority(Enum job, float p, bool set = false)
  {
    if (job is FJob)
    {
      FJob tmpJob = (FJob)job;
      if (set) JobDic[tmpJob].Priority = p;
      else
      {
        if (JobDic.TryGetValue(tmpJob, out var value))
          JobDic[tmpJob].Priority += p;
      }
      JobChanged = new KeyValuePair<FJob, jobDicValue>(tmpJob, JobDic[tmpJob]);
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

  public void SetJobIsAble(Enum job, bool set)
  {
    FJob tmpJob = (FJob)job;
    if (JobDic.TryGetValue(tmpJob, out var value))
      JobDic[tmpJob].IsAble = set;
  }

  public void ResetJobIsAble()
  {
    foreach(var job in JobDic)
    {
      job.Value.IsAble = true;
    }

    jobSystem.supplyTargets.Clear();
  }

  public float GetJobPriority(Enum job)
  {
    if(job is FJob)
    {
      if (JobDic.TryGetValue((FJob)job, out var value))
        return value.Priority;
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
        switch(job)
        {
          case FJob.Supply:
          case FJob.Store:
          case FJob.Make:
          case FJob.Research:
            PlayAnimation(CreatureData.Job); break;
          default: PlayAnimation(Skills[0].AnimName); break;
        }
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
  public Coroutine _coai;

  public void RestartCo()
  {
    if (_coai != null)
    {
      _coai = StartCoroutine(CoUpdateAI());
    }
  }

  public void CancelCo()
  {
    if (_coai != null)
    {
      StopCoroutine(_coai);
      
    }
  }

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
      //if(job is FPersonalJob.Sleepy)
      //{
      //  LerpCellPosCompleted = true;
      //  if (_coai != null) { CancelCo(); StopAnimation(); }
      //  SpriteRenderer.sprite = Managers.Resource.Load<Sprite>("warrior-sleep");
      //  this.transform.position = chaseTarget.transform.position;
      //  chaseTarget.OnDamaged(this);
      //  return;
      //}
      //else if(job is FJob.Supply)
      //{
        
      //}
      

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

        //chaseTarget = null;
        CreatureState = FCreatureState.Skill;
        //ResetJob();
        
      }
      else if(result == FFindPathResults.Fail_MoveTo)
      {
        ResetJob();
        //chaseTarget = null;
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

  #endregion

  #region Job
  public virtual void ResetJob()
  {
    onWork = false;
    Target.Worker = null;
    Target = null;
    supplyTarget = null;

    CreatureMoveState = FCreatureMoveState.None;
    CreatureState = FCreatureState.Idle;
  }

  protected virtual void JobMake(float distance)
  {
    var targetScr = Target as BuildObject;
    if(targetScr != null)
    {
      if(supplyTarget != null)
      {
        ChaseOrAttackTarget(100, distance, supplyTarget);
      }
      else
      {
        ChaseOrAttackTarget(100, distance);
      }
    }

  }

  protected virtual void JobPlow(float distance)
  {
    var targetScr = Target as Env;
    if (targetScr != null)
    {
      if (!targetScr.harvestIsReady)
      {
        SetJobIsAble(job, false);
        ResetJob();
      }
      else
      {
        ChaseOrAttackTarget(100, distance);
      }
    }
    else
    {
      SetJobIsAble(job, false);
      ResetJob();
    }
  }

  protected virtual void JobStore(float distance)
  {
    supplyTarget = jobSystem.CurrentRootJob;
    var targetScr = Target as Storage;
    //if(targetScr == null)
    //{
    //  SetJobIsAble(job, false);
    //  ResetJob();
    //  return;
    //}

    if (supplyTarget != null)
    {
      ChaseOrAttackTarget(100, distance, supplyTarget);
    }
    else
    {
      //들고 있는게 있어야 함 .. 없는데 이쪽으로 넘어오면 욕구 취소로 다음사람에게 넘김?
      if (CurrentSupply > 0 && targetScr.CurCapacity < targetScr.MaxCapacity) ChaseOrAttackTarget(100, distance);
      else
      {
        SetJobIsAble(job, false);
        ResetJob();
      }
    }

  }

  protected virtual void JobSupply(float distance)
  {
    var targetScr = Target as BuildObject;
    if (!targetScr.setFlag)
    {
      SetJobIsAble(job, false);
      ResetJob();
      return;
    }
    else SetJobIsAble(job, true);
    
    if (targetScr != null)
    {
      foreach (var item in targetScr.buildNeedList)
      {
        if (SearchHaveList(item.Key))
        {
          ChaseOrAttackTarget(100, distance);
        }
        else
        {
          // 들고 있지 않다면 주변에서 찾아봄
          // 1. 상자
          // 상자를 서치해서 그 안에 있는 아이템을 가져옴 
          // 2. 땅에 떨어진 것들
          // 땅에 떨어진 루팅할 수잇는 아이템들 중에서 id가 같은것을 찾아봄

          //2. 방법
          supplyTarget = jobSystem.CurrentRootJob;
          var supplyTargetScr = supplyTarget as ItemHolder;
          if(supplyTarget != null && CurrentSupply + supplyTargetScr.mass < SupplyCapacity && item.Key == supplyTarget.dataTemplateID && supplyTargetScr.isDropped)
          {
            ChaseOrAttackTarget(100, distance, supplyTarget);
          }
          else
          {
            SetJobIsAble(job, false);
            ResetJob();
          }
        }
      }
    }
    else
    {
      SetJobIsAble(job, false);
      ResetJob();
    }

  }

  public virtual void JobSleepy(float distance)
  {
    ChaseOrAttackTarget(100, distance);

    return;
  }

  public virtual void JobHungry(float distance)
  {
    ChaseOrAttackTarget(100, distance);

    return;
  }

  #endregion
}
