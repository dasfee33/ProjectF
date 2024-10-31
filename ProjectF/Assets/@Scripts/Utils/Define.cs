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

  public enum FTool
  {
    Plow,

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
    Env,
  }

  public enum FEnvType
  {
    None,
    Tree,
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

  public enum FEnvState
  {
    None,
    Idle,
    Hurt,
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
    CollectEnv,
    //TODO

  }

  //TEMP
  public enum FJob
  {
    None,
    Hungry, // 식욕
    Sleepy, // 수면욕
    Excretion, // 배설욕
    Mood, //기분
    Attack,
    Rescue,
    Toggle,
    Medic,
    Array,
    Cook,
    Deco,
    Research,
    Machine,
    Plow,
    Breed,
    Make,
    Dig,
    Logging,
    Supply,
    Store,
    //TODO,
  }

  public enum FFindPathResults
  {
    Fail_LerpCell,
    Fail_NoPath,
    Fail_MoveTo,
    Success,
  }

  public enum FCellCollisionTypes
  {
    None,
    SemiWall,
    Wall,
  }

  public const char MAP_TOOL_WALL = '0';
  public const char MAP_TOOL_NONE = '1';
  public const char MAP_TOOL_SEMI_WALL = '2';

  public const int CREATURE_WARRIOR_DATAID = 1;
  public const int ENV_TREE_NORMAL1 = 100000;
  public const int ENV_TREE_NORMAL2 = 100001;

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





