using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{
  public override bool Init()
  {
    if (base.Init() == false) return false;

    CreatureType = FCreatureType.WARRIOR;
    CreatureState = FCreatureState.Idle;
    Speed = 2.0f;

    return true;
  }
}
