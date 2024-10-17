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
}
