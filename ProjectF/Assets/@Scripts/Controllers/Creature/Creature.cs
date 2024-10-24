using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static Define;

public class Creature : BaseObject
{
  public float Speed { get; protected set; } = 1.0f;
  public FCreatureType CreatureType { get; protected set; } = FCreatureType.None;

  private string[] jobNames = Enum.GetNames(typeof(FJob));

  public Dictionary<string, int> JobDic = new Dictionary<string, int>();
  public Dictionary<string, int> jobToggleDic = new Dictionary<string, int>();

  protected FCreatureState creatureState = FCreatureState.None;
  public virtual FCreatureState CreatureState
  {
    get { return creatureState; }
    set
    {
      if (creatureState != value)
      {
        creatureState = value;
        UpdateAnimation(CreatureType);
      }
    }
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    ObjectType = FObjectType.Creature;
    foreach(string job in jobNames)
    {
      JobDic.Add(job, 0);
    }
    foreach(string job in jobNames)
    {
      jobToggleDic.Add(job, 0);
    }

    return true;
  }

  protected override void UpdateAnimation(FCreatureType type)
  {
    switch(type)
    {
      case FCreatureType.WARRIOR:
        PlayWarriorAnimation();
        break;
      default: break;
    }
  }

  private void PlayWarriorAnimation()
  {
    switch(CreatureState)
    {
      case FCreatureState.Idle:
        PlayAnimation(AnimName.WARRIOR_IDLE);
        break;
      case FCreatureState.Move:
        PlayAnimation(AnimName.WARRIOR_RUN);
        break;
      case FCreatureState.Dead:
        PlayAnimation(AnimName.WARRIOR_DEATH);
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

  protected virtual void UpdateIdle() { }
  protected virtual void UpdateMove() { }
  protected virtual void UpdateSkill() { }
  protected virtual void UpdateDead() { }

  protected FJob SelectJob(/*Func<BaseObject, bool> func = null*/)
  {
    FJob target = FJob.None;

    var dic = JobDic.Aggregate((a, b) => a.Value > b.Value ? a : b);

    if(Enum.TryParse(typeof(FJob), dic.Key, out var job))
    {
      target = (FJob)job;
    }
    if (dic.Value <= 0) target = FJob.None;

    return target;
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

}
