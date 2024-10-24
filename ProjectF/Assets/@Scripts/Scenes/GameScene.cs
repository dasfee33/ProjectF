using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
  public override bool Init()
  {
    if (base.Init() == false) return false;

    SceneType = Define.FScene.GameScene;

    Managers.Map.LoadMap("BaseMap");

    Player warrior = Managers.Object.Spawn<Player>("Warrior", new Vector2(-5, 1));
    Player warrior2 = Managers.Object.Spawn<Player>("Warrior", new Vector2(-5, 0));
    //warrior.CreatureState = FCreatureState.Move;

    CameraController camera = Camera.main.gameObject.GetOrAddComponent<CameraController>();
    camera.Target = warrior;

    return true;
  }
}
