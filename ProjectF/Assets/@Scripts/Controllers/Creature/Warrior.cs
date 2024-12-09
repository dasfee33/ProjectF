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

  public override bool Init()
  {
    if (base.Init() == false)
      return false;


    CreatureType = FCreatureType.Warrior;
    previousPos = transform.position;

    //Map
    Collider.isTrigger = false;
    RigidBody.simulated = false;

    //SetOrAddJobPriority(FJob.Logging, 20);

    Managers.GameDay.dayChanged -= ResetJobIsAble;
    Managers.GameDay.dayChanged += ResetJobIsAble;

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
      if (CreatureMoveState == FCreatureMoveState.Job) CreatureState = FCreatureState.Move;

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

        switch(job)
        {
          case FJob.Store: JobStore(MinActionDistance); break;
          case FJob.Supply: JobSupply(MinActionDistance); break;
          case FJob.Make: JobMake(MinActionDistance); break;
          case FJob.Plow: JobPlow(MinActionDistance); break;

          case FPersonalJob.Sleepy: JobSleepy(MinActionDistance); break;
          case FPersonalJob.Hungry: JobHungry(MinActionDistance); break;
          default: ChaseOrAttackTarget(100, MinActionDistance); break;
        }

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
    //SetOrAddJobPriority(FJob.Store, 1f);

    ////TEMP
    //SetOrAddJobPriority(FJob.Supply, 20f);
    //SetOrAddJobPriority(FJob.Make, 20f);
  }

  public override void OnAnimEventHandler()
  {
    if (Target.IsValid() == false) return;
    
    if(job is FJob.Store || job is FJob.Supply)
    {
      if (supplyTarget != null)
      {
        supplyTarget.OnDamaged(this);
        CreatureState = FCreatureState.Idle;
        return;
      }
      
    }

    Target.OnDamaged(this);
  }

  public void OnAnimIsEnd()
  {

    CreatureState = FCreatureState.Idle;

  }

  #endregion
}

