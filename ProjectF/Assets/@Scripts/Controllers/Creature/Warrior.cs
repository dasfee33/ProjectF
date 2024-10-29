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
            UpdateAITick = 0.0f;
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
    SetOrAddJobPriority(FJob.Cook, 10);
    SetOrAddJobPriority(FJob.Logging, 20);
    SetOrAddJobPriority(FJob.Toggle, 30);

    StartCoroutine(CoUpdateAI());

    return true;
  }

  #region AI
  public float SearchDistance { get; private set; } = 8.0f;
  public float ActionDistance { get; private set; } = 1.0f;

  Vector3 _destPos;
  Vector3 _initPos;

  protected override void UpdateIdle()
  {
    Debug.Log("Idle");

    // Patrol
    {
      int patrolPercent = 10;
      int rand = Random.Range(0, 100);
      if (rand <= patrolPercent)
      {
        _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
        CreatureState = FCreatureState.Move;
        return;
      }
    }

    //Job selection
    {
      job = SelectJob();
      if(job != FJob.None)
      {
        //TEMP
        //TODO SEARCH
        Target = GameObject.Find(System.Enum.GetName(typeof(FJob), job)).GetOrAddComponent<BaseObject>();
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
      FindPathAndMoveToCellPos(_destPos, 3);

      if(LerpCellPosCompleted)
      {
        CreatureState = FCreatureState.Idle;
        return;
      }
    }
    else
    {
      if (CreatureMoveState == FCreatureMoveState.Job)
      {
        FFindPathResults result = FindPathAndMoveToCellPos(Target.transform.position, 100);

        if (LerpCellPosCompleted)
        {
          SetOrAddJobPriority(job, 0, true);
          CreatureMoveState = FCreatureMoveState.None;
          CreatureState = FCreatureState.Idle;
          Target = null;

          StartWait(2.0f);
          return;
        }
        return;
      }
    }

    

    //if (_target == null)
    //{
    //  // Patrol or Return
    //  Vector3 dir = (_destPos - transform.position);
    //  float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
    //  transform.TranslateEx(dir.normalized * moveDist);

    //  if (dir.sqrMagnitude <= 0.01f)
    //  {
    //    CreatureState = FCreatureState.Idle;
    //  }
    //}
    //else
    //{
    //  //// Chase
    //  Vector3 dir = (_target.transform.position - transform.position);
    //  float distToTargetSqr = dir.sqrMagnitude;
    //  float attackDistanceSqr = ActionDistance * ActionDistance;

    //  if (distToTargetSqr < attackDistanceSqr)
    //  {
    //    // 범위 내로 들어왔으면 작업
    //    JobDic[System.Enum.GetName(typeof(FJob), job)] = 0;
    //    _target = null;
    //    CreatureState = FCreatureState.Idle;
    //    CreatureMoveState = FCreatureMoveState.None;
    //    StartWait(2.0f);
    //  }
    //  else
    //  {
    //    // 범위 밖이라면 추적.
    //    float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
    //    transform.TranslateEx(dir.normalized * moveDist);

    //    // 너무 멀어지면 포기.
    //    //float searchDistanceSqr = SearchDistance * SearchDistance;
    //    //if (distToTargetSqr > searchDistanceSqr)
    //    //{
    //    //  _destPos = _initPos;
    //    //  _target = null;
    //    //  CreatureState = ECreatureState.Move;
    //    //}
    //  }
    //}
  }


  #endregion
}
