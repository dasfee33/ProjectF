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
    Managers.RandomSeedGenerate.GenerateMaps();

    Warrior warrior = Managers.Object.Spawn<Warrior>(Vector3.zero, CREATURE_WARRIOR_DATAID, "Warrior");
    Warrior warrior2 = Managers.Object.Spawn<Warrior>(Vector3.zero, CREATURE_WARRIOR_DATAID, "Warrior");

    //Env env1 = Managers.Object.Spawn<Env>(new Vector3(-1, 0), ENV_TREE_NORMAL1, "Tree1");
    //Env env2 = Managers.Object.Spawn<Env>(new Vector3(1, 0), ENV_TREE_NORMAL2, "Tree2");

    Managers.Map.MoveTo(warrior, new Vector3Int(-5, 1), true);
    Managers.Map.MoveTo(warrior2, new Vector3Int(-5, 0), true);
    //warrior.CreatureState = FCreatureState.Move;

    CameraController camera = Camera.main.GetComponent<CameraController>();
    camera.virtualCam.Follow = warrior.transform;
    //camera.transform.position = new Vector3(0, 0, -2);

    return true;
  }
}
