using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
  public float Speed { get; protected set; } = 1.0f;
  public FCreatureType CreatureType { get; protected set; } = FCreatureType.None;

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

}
