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
    Warrior warrior3 = Managers.Object.Spawn<Warrior>(Vector3.zero, CREATURE_WARRIOR_DATAID, "Warrior");
    //Warrior warrior4 = Managers.Object.Spawn<Warrior>(Vector3.zero, CREATURE_WARRIOR_DATAID, "Warrior");
    //Warrior warrior5 = Managers.Object.Spawn<Warrior>(Vector3.zero, CREATURE_WARRIOR_DATAID, "Warrior");

    //Structure toilet = Managers.Object.Spawn<Structure>(Vector3.zero + Vector3.right * 3, STRUCTURE_TOILET_NORMAL, "Toilet");
    //Structure toilet2 = Managers.Object.Spawn<Structure>(Vector3.zero + Vector3.right * 4, STRUCTURE_TOILET_NORMAL, "Toilet");
    //Structure bed1 = Managers.Object.Spawn<Structure>(Vector3.zero + Vector3.left, STRUCTURE_BED_NORMAL, "Bed1");
    //Structure bed2 = Managers.Object.Spawn<Structure>(Vector3.zero + Vector3.left + new Vector3(-1, 0), STRUCTURE_BED_NORMAL, "Bed1");
    //Structure bed3 = Managers.Object.Spawn<Structure>(Vector3.zero + Vector3.left + new Vector3(-2, 0), STRUCTURE_BED_NORMAL, "Bed1");
    //Env env1 = Managers.Object.Spawn<Env>(new Vector3(-1, 0), ENV_TREE_NORMAL1, "Tree1");
    //Env env2 = Managers.Object.Spawn<Env>(new Vector3(1, 0), ENV_TREE_NORMAL2, "Tree2");

    Managers.Map.MoveTo(warrior, new Vector3Int(-5, 0), true);
    Managers.Map.MoveTo(warrior2, new Vector3Int(-5, 1), true);
    Managers.Map.MoveTo(warrior3, new Vector3Int(-5, -1), true);
    //Managers.Map.MoveTo(warrior4, new Vector3Int(-4, 1), true);
    //Managers.Map.MoveTo(warrior5, new Vector3Int(-4, 0), true);
    //warrior.CreatureState = FCreatureState.Move;

    CameraController camera = Camera.main.GetComponent<CameraController>();
    if (Managers.Object.Creatures.Count > 0)
    {
      var count = Managers.Object.Creatures.Count;
      camera.virtualCam.Follow = Managers.Object.Creatures[camera.targetNum % count].transform;

    }
    //camera.transform.position = new Vector3(0, 0, -2);

    return true;
  }
}
