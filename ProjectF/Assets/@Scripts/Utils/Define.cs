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

  // 캐릭터 기본 상태
  public enum FCreatureState
  {
    None,
    Idle,
    Move,
    Skill,
    Dead,
  }

  // 캐릭터 세부 상태
  public enum FCreatureMoveState
  {
    None,
    Job,
    Hungry,
    Tremble, //떨림
    Act,
    //TODO

  }

  //TEMP
  public enum FJob
  {
    None,
    Attack,
    Rescue,
    Toggle,
    Medic,
    Array,
    Cook,
    Deco,
    Research,
    Machine,
    Plant,
    Breed,
    Make,
    Dig,
    Supply,
    Store,
    //TODO,
  }
}

public class JobPriority
{
  public int H { get; set; }
  public Define.FJob Job { get; set; }

  public JobPriority(int h, Define.FJob job)
  {
    H = h;
    Job = job;
  }

  public void JobPlus(int a)
  {
    H += a;
  }

  public void JobMinus(int a)
  {
    H -= a;
  }

  public int JobScore()
  {
    return H;
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
