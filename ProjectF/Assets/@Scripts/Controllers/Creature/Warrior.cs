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


    CreatureType = FCreatureType.WARRIOR;
    previousPos = transform.position;

    //Map
    Collider.isTrigger = false;
    RigidBody.simulated = false;

    //TEST
    //SetOrAddJobPriority(FJob.Cook, 10);
    SetOrAddJobPriority(FJob.Logging, 20);
    //SetOrAddJobPriority(FJob.Toggle, 30);

    //jobSystem += jobChanged

    StartCoroutine(CoUpdateAI());
    StartCoroutine(CoUpdateState());

    return true;
  }

  #region AI
  public float SearchDistance { get; private set; } = 8.0f;
  public float MinActionDistance { get; private set; } = 0.4f;
  //public float MaxActionDistance { get; private set; } = 1f;

  Vector3 _destPos;
  //Vector3 _initPos;

  protected override void UpdateIdle()
  {
    Debug.Log("Idle");

    // Patrol
    {
      int patrolPercent = 10;
      int rand = Random.Range(0, 100);
      if (rand <= patrolPercent)
      {
        if (CreatureState == FCreatureState.Skill) return;
        _destPos = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        CreatureState = FCreatureState.Move;
        return;
      }
    }

    //Job selection
    {
      job = SelectJob();
      Target = jobSystem.target;
      if(job != FJob.None)
      {
        CreatureMoveState = FCreatureMoveState.Job;
        CreatureState = FCreatureState.Move;
      }

    }
    //CreatureState = FCreatureState.Move;
  }

  protected override void UpdateMove()
  {
    Debug.Log("Move");

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

        if (Target.ObjectType == FObjectType.Env)
        {
          ChaseOrAttackTarget(100, MinActionDistance);//, MaxActionDistance);

          if (Target.IsValid() == false)
          {
            onWork = false;
            Target = null;

            CreatureMoveState = FCreatureMoveState.None;
            CreatureState = FCreatureState.Idle;
          }

          //if (LerpCellPosCompleted)
          //{
          //  //StartWait(2.0f);
          //  //Managers.Object.Despawn(Target);
          //  if(Target.IsValid() == false)
          //  {
             
          //  }

          //  return;
          //}
        }
        return;
      }
    }
  }

  protected override void UpdateSkill()
  {
    var dir = (Target.transform.position - this.transform.position).normalized;
    if (dir.x < 0) LookLeft = true;
    else LookLeft = false; 

    if (Target.IsValid() == false)
    {
      CreatureState = FCreatureState.Move;
      return;
    }
  }

  protected override void UpdateState()
  {
    SetOrAddJobPriority(FJob.Hungry, 10f);
    SetOrAddJobPriority(FJob.Sleepy, 10f);
    SetOrAddJobPriority(FJob.Excretion, 10f);
    SetOrAddJobPriority(FJob.Store, 10f);
  }

  public override void OnAnimEventHandler()
  {
    if (Target.IsValid() == false) return;

    Target.OnDamaged(this);
  }

  public void OnAnimIsEnd()
  {
    //if (CreatureState != FCreatureState.Move) return;  

    CreatureState = FCreatureState.Idle;

  }

  #endregion
}

