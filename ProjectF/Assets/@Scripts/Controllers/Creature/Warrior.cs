using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Warrior : Creature
{
  public override FCreatureState CreatureState
  {
    get { return base.CreatureState; }
    set
    {
      if (creatureState != value)
      {
        base.CreatureState = value;
        switch (value)
        {
          case FCreatureState.Idle:
            UpdateAITick = 0.5f;
            break;
          case FCreatureState.Move:
            UpdateAITick = 0.0f;
            break;
          case FCreatureState.Skill:
            UpdateAITick = Skills[0].CoolTime;
            break;
          case FCreatureState.Dead:
            UpdateAITick = 1.0f;
            break;
        }
      }
    }
  }

  FCreatureMoveState creatureMoveState = FCreatureMoveState.None;
  public FCreatureMoveState CreatureMoveState
  {
    get { return creatureMoveState; }
    set
    {
      creatureMoveState = value;
    }
  }

  public override bool Init()
  {
    if (base.Init() == false)
      return false;


    CreatureType = FCreatureType.Warrior;
    previousPos = transform.position;

    //Map
    Collider.isTrigger = false;
    RigidBody.simulated = false;

    //TEST
    //SetOrAddJobPriority(FJob.Cook, 10);
    SetOrAddJobPriority(FJob.Logging, 20);
    //SetOrAddJobPriority(FJob.Toggle, 30);

    //jobSystem += jobChanged

    _coai = StartCoroutine(CoUpdateAI());
    StartCoroutine(CoUpdateState());

    return true;
  }

  #region AI
  public float SearchDistance { get; private set; } = 8.0f;
  public float MinActionDistance { get; private set; } = 0.5f;
  //public float MaxActionDistance { get; private set; } = 1f;

  Vector3 _destPos;
  //Vector3 _initPos;

  protected override void UpdateIdle()
  {

    // Patrol
    {
      int patrolPercent = 10;
      int rand = Random.Range(0, 100);
      if (rand <= patrolPercent)
      {
        if (CreatureState == FCreatureState.Skill/* || CreatureMoveState == FCreatureMoveState.Job*/) return;
        _destPos = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        CreatureState = FCreatureState.Move;
        return;
      }
    }

    //Job selection
    {
      job = SelectJob();
      if(job is FJob and not FJob.None)
      {
        Target = jobSystem.target;
        CreatureMoveState = FCreatureMoveState.Job;
        CreatureState = FCreatureState.Move;
      }
      else if(job is FPersonalJob and not FPersonalJob.None)
      {
        Target = ppSystem.target;
        if (Target.onWorkSomeOne) return;
        CreatureMoveState = FCreatureMoveState.Job;
        CreatureState = FCreatureState.Move;
      }
    }
    //CreatureState = FCreatureState.Move;
  }

  protected override void UpdateMove()
  {

    if (Target.IsValid() == false)
    {
      //일하러 가고 잇는데 일이 끝났거나 없어진 경우 => 다시 서치
      //if(onWork)
      //{
      //  Target = null;
      //  CreatureMoveState = FCreatureMoveState.None;
      //  CreatureState = FCreatureState.Idle;
      //  onWork = false;
      //}
      FindPathAndMoveToCellPos(_destPos, 3);

      if(LerpCellPosCompleted)
      {
        Target = null;
        //StartWait(2.0f);
        CreatureMoveState = FCreatureMoveState.None;
        CreatureState = FCreatureState.Idle;
        return;
      }
    }
    else
    {
      if (CreatureMoveState == FCreatureMoveState.Job)
      {
        onWork = true;

        //FIXME
        if (Target.onWorkSomeOne) { CreatureState = FCreatureState.Idle; return; }

        if (job is FJob.Store)
        {
          supplyTarget = jobSystem.CurrentRootJob;
          if (supplyTarget != null && CurrentSupply < SupplyCapacity)
          {
            ChaseOrAttackTarget(100, MinActionDistance, supplyTarget);
          }
          else
          {
            //들고 있는게 있어야 함 .. 없는데 이쪽으로 넘어오면 욕구 취소로 다음사람에게 넘김?
            if (CurrentSupply > 0) ChaseOrAttackTarget(100, MinActionDistance);
            else
            {
              //FIXME
              SetOrAddJobPriority(job, 0, true);
              onWork = false;
              Target = null;

              CreatureMoveState = FCreatureMoveState.None;
              CreatureState = FCreatureState.Idle;
            }
          }
        }
        //else if(job is FPersonalJob.Sleepy)
        //{
        //  if (_coai != null) StopCoroutine(_coai);
        //  SpriteRenderer.sprite = Managers.Resource.Load<Sprite>("warrior-sleep");
        //}
        else ChaseOrAttackTarget(100, MinActionDistance);//, MaxActionDistance);

        if (Target.IsValid() == false)
        {
          onWork = false;
          Target = null;

          CreatureMoveState = FCreatureMoveState.None;
          CreatureState = FCreatureState.Idle;
        }
        return;
      }
      
    }
  }

  protected override void UpdateSkill()
  {
    //var dir = (Target.transform.position - this.transform.position).normalized;
    //if (dir.x < 0) LookLeft = true;
    //else LookLeft = false; 

    if (Target.IsValid() == false)
    {
      CreatureState = FCreatureState.Move;
      return;
    }
  }

  protected override void UpdateState()
  {
    if (Managers.GameDay.currentTime >= FCurrentTime.BeforeSunset)
      SetOrAddJobPriority(FPersonalJob.Sleepy, 5f);
    SetOrAddJobPriority(FJob.Store, 1f);
  }

  public override void OnAnimEventHandler()
  {
    if (Target.IsValid() == false) return;
    if(job is FJob.Store)
    {
      if (supplyTarget != null && CurrentSupply < SupplyCapacity)
      {
        supplyTarget.OnDamaged(this);
        return;
      }
    }

    Target.OnDamaged(this);
  }

  public void OnAnimIsEnd()
  {
    //if (CreatureState != FCreatureState.Move) return;  

    CreatureState = FCreatureState.Idle;

  }

  #endregion
}

