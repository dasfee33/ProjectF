using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
  public enum FSizeUnits
  {
    Byte, KB, MB, GB
  }

  public enum FLanguage
  {
    Korean,
    English,
    Japanese,
    French,
    SChinese,
    TChinese,
  }

  public enum FResearchType
  {
    None,
    Basic,
    //TODO
  }

  public enum FCurrentTime
  {
    Dawn,
    Day,
    BeforeSunset,
    Night,
  }

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

  public enum FJobSelectEvent
  {
    Never,
    DownDown,
    Down,
    None,
    Up,
    UpUp,
  }

  public enum FTool
  {
    Plow,
    Build,
  }

  public enum FJobPhase
  {
    None,
    Start,
    On,
    Done,
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
    Structure,
    ItemHolder,
    BuildObject,
  }

  public enum FEnvType
  {
    None,
    Tree,
    Rock,
    Plant,
    Trunk,
  }

  public enum FStructureType
  {
    None,
    Pipe,
    Furniture,
    Base,
    Electronic,
    Station,
    PlowBowl,
    BuildObject,
  }

  public enum FStructureSubType
  {
    None,
    Toilet,
    EatingTable,
    Storage,
    Bed,
    Station,
    BuildObject,
    PlowBowl,
  }

  public enum FCreatureType
  {
    None,
    Warrior,
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

  public enum FStructureState
  {
    None,
    Idle,
    WorkStart,
    Work,
    WorkEnd,
  }

  // 캐릭터 세부 상태
  public enum FCreatureMoveState
  {
    None,
    Patrol,
    Job,
    Hungry,
    Tremble, //떨림
    Act,
    CollectEnv,
    //TODO

  }

  //TEMP
  public enum FPersonalJob
  {
    None,
    Hungry, // 식욕
    Sleepy, // 수면욕
    Excretion, // 배설욕
  }

  public enum FJob
  {
    None,
    Attack,
    Rescue,
    //Toggle,
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

  public enum FItemGrade
  {
    None,
    Normal,
    Rare,
    Epic,
    Legendary,
  }

  public enum FItemGroupType
  {
    None,
    Equipment,
    Consumable,
  }

  public enum FItemType
  {
    None,
    Architecture,
    Cook,
    Plow,
  }

  public enum FItemSubType
  {
    None,
    Seed,
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

  public enum FCellObjCollisionTypes
  {
    None,
    SemiWall,
    Wall,
    Tree,
    Rock,
    Gayser,
    KickPlant,
  }

  //safe area 대응 
  public enum FSetUISafeArea
  {
    None = 0,
    All = Vertical | Horizontal,
    Vertical = Top | Bottom,
    Horizontal = Left | Right,

    Top = 0x01,
    Bottom = 0x02,
    Left = 0x04,
    Right = 0x08,
  }

  public const char MAP_TOOL_WALL = '0';
  public const char MAP_TOOL_NONE = '1';
  public const char MAP_TOOL_SEMI_WALL = '2';

  public const char MAPOBJ_TOOL_WALL = '0';
  public const char MAPOBJ_TOOL_SEMI_WALL = '1';
  public const char MAPOBJ_TOOL_TREE = '2';
  public const char MAPOBJ_TOOL_ROCK = '3';
  public const char MAPOBJ_TOOL_GAYSER = '4';
  public const char MAPOBJ_TOOL_KICKPLANT = '5';

  public const int RESEARCH_START = 4000;

  public const int CREATURE_WARRIOR_DATAID = 1;
  public const int ENV_TREE_NORMAL1 = 100000;
  public const int ENV_TREE_NORMAL2 = 100001;
  public const int ENV_ROCK_NORMAL1 = 100002;
  public const int ENV_PLANT_KICK = 100003;
  

  public const int STRUCTURE_TOILET_NORMAL = 100;
  public const int STRUCTURE_COOKTABLE_NORMAL = 101;
  public const int STRUCTURE_BED_NORMAL = 102;
  public const int STRUCTURE_CHEST_NORMAL = 103;
  public const int STRUCTURE_STATION_NORMAL = 104;
  public const int STRUCTURE_PLOWBOWL_NORMAL = 105;

  public const string STRUCTURE_BUILD = "_build";
  public const string INFODESC = "infodesc";
  public const string INFOTEXT = "infotext";
  public const string INFOTITLE = "Title";
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

public static class COLOR
{
  public static Vector4 DAY = new Vector4(1, 1, 1, 1);
  public static Vector4 BEFORESUNSET = new Vector4(0.9f, 0.6f, 0.1f, 1);
  public static Vector4 NIGHT = new Vector4(0.5f, 0.4f, 0.4f);

  public static Vector4 SMOKERED = new Vector4(1, 0, 0, 0.5f);
  public static Vector4 SMOKEWHITE = new Vector4(1, 1, 1, 0.5f);
}





