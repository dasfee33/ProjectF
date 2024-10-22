using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{
  //private Vector2 _moveDir = Vector2.zero;

  //public override bool Init()
  //{
  //  if (base.Init() == false) return false;

  //  CreatureType = FCreatureType.WARRIOR;
  //  CreatureState = FCreatureState.Idle;
  //  Speed = 2.0f;

  //  Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
  //  Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;


  //  return true;
  //}

  //private void Update()
  //{
  //  transform.TranslateEx((_moveDir + Vector2.right) * Time.deltaTime * Speed);

  //}

  //private void HandleOnMoveDirChanged(Vector2 dir)
  //{
  //  _moveDir = dir;
  //}

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
    CreatureState = FCreatureState.Idle;
    Speed = 3.0f;
    previousPos = transform.position;

    //TEST
    jobToggleDic["Cook"] = 10;
    jobToggleDic["Deco"] = 20;
    jobToggleDic["Toggle"] = 30;

    foreach(string jobtoggle in jobToggleDic.Keys)
    {
      if(JobDic.TryGetValue(jobtoggle, out int value))
      {
        value += jobToggleDic[jobtoggle];
        JobDic[jobtoggle] = value;
      }
    }

    StartCoroutine(CoUpdateAI());

    return true;
  }

  #region AI
  public float SearchDistance { get; private set; } = 8.0f;
  public float AttackDistance { get; private set; } = 4.0f;
  Creature _target;
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
    //CreatureState = FCreatureState.Move;
  }

  protected override void UpdateMove()
  {
    Debug.Log("Move");

    if (_target == null)
    {
      // Patrol or Return
      Vector3 dir = (_destPos - transform.position);
      float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
      transform.TranslateEx(dir.normalized * moveDist);

      if (dir.sqrMagnitude <= 0.01f)
      {
        CreatureState = FCreatureState.Idle;
      }
    }
    else
    {
      //// Chase
      //Vector3 dir = (_target.transform.position - transform.position);
      //float distToTargetSqr = dir.sqrMagnitude;
      //float attackDistanceSqr = AttackDistance * AttackDistance;

      //if (distToTargetSqr < attackDistanceSqr)
      //{
      //  // 공격 범위 이내로 들어왔으면 공격.
      //  CreatureState = ECreatureState.Skill;
      //  StartWait(2.0f);
      //}
      //else
      //{
      //  // 공격 범위 밖이라면 추적.
      //  float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
      //  transform.TranslateEx(dir.normalized * moveDist);

      //  // 너무 멀어지면 포기.
      //  float searchDistanceSqr = SearchDistance * SearchDistance;
      //  if (distToTargetSqr > searchDistanceSqr)
      //  {
      //    _destPos = _initPos;
      //    _target = null;
      //    CreatureState = ECreatureState.Move;
      //  }
      //}
    }
  }


  #endregion
}

