using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
  public enum FScene
  {
    UnKnown,
    TitleScene,
    GameScene,
  }

  public enum FUIEvent
  {
    Click,
    PointerDown,
    PointerUp,
    Drag,
  }

  public enum FSound
  {
    Bgm,
    Effect,
    Max,
  }

  public enum FObjectType
  {
    None,
    Creature,
    Projectile,
    Prop,
  }

  public enum FCreatureType
  {
    None,
    WARRIOR,
    Npc,
  }

  public enum FCreatureState
  {
    None,
    Idle,
    Move,
    Skill,
    Dead,
  }
}

public static class AnimName
{
  public const string WARRIOR_IDLE = "warrior idle";
  public const string WARRIOR_HURT = "warrior hurt";
  public const string WARRIOR_DEATH = "warrior death";
  public const string WARRIOR_RUN = "warrior run";
  public const string WARRIOR_SWING1 = "warrior single swing1";
  public const string WARRIOR_SWING3 = "warrior single swing3";
  public const string WARRIOR_COMBOATTACK = "warrior full combo atk";
  
}
