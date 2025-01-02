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
    //Managers.RandomSeedGenerate.GenerateMaps();

    //저장 된 데이터가 있습니다.
    if(Managers.Game.LoadFlag)
    {
      ParseSaveData();
    }
    else
    {
      Managers.Game.InitGame();
    }

    //CameraController camera = Camera.main.GetComponent<CameraController>();
    //if (Managers.Object.Creatures.Count > 0)
    //{
    //  var count = Managers.Object.Creatures.Count;
    //  camera.virtualCam.Follow = Managers.Object.Creatures[camera.targetNum % count].transform;

    //}
    //camera.transform.position = new Vector3(0, 0, -2);


    return true;
  }

  public void ParseSaveData()
  {
    var creatureSaveData = Managers.Game.SaveData.creatureSaveData;
    var envSaveData = Managers.Game.SaveData.envSaveData;
    var structureData = Managers.Game.SaveData.structSaveData;
    var ItemHolderData = Managers.Game.SaveData.itemHolderSaveData;
    var ItemData = Managers.Game.SaveData.itemSaveData;

    foreach (var env in envSaveData)
    {
      Managers.Object.Spawn<Env>(new Vector3(env.posX, env.posY), env.dataID, env.name);
    }

    foreach (var structure in structureData.toiletSaveData)
    {
      Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name);
    }
    foreach (var structure in structureData.eatingTableSaveData)
    {
      Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name);
    }
    foreach (var structure in structureData.storageSaveData)
    {
      var storage = Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name) as Storage;
      foreach (var s in structure.storageItem)
        storage.AddCapacity(s.id, s.mass, s.label);
    }
    foreach (var structure in structureData.bedSaveData)
    {
      Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name);
    }
    foreach (var structure in structureData.stationSaveData)
    {
      Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name);
    }
    foreach (var structure in structureData.buildObjectFSaveData)
    {
      Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name);
    }
    foreach (var structure in structureData.plowBowlSaveData)
    {
      Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name);
    }
    foreach (var structure in structureData.plowSoilSaveData)
    {
      Managers.Object.Spawn<Structure>(new Vector3(structure.posX, structure.posY), structure.dataID, structure.name);
    }

    foreach (var ItemHolder in ItemHolderData)
    {
      var item = Managers.Object.Spawn<ItemHolder>(new Vector3(ItemHolder.posX, ItemHolder.posY), ItemHolder.dataID, addToCell: false);
      item.mass = ItemHolder.mass;
      item.stack = ItemHolder.stack;
      item.isDropped = ItemHolder.isDropped;
    }

    foreach (var item in ItemData)
    {
      Managers.Object.AddItem(item.dataID, item.mass);
    }

    foreach (var creatureData in creatureSaveData)
    {
      switch (creatureData.type)
      {
        case (int)FCreatureType.Warrior:
          //FIXME
          Vector3Int pos = new Vector3Int((int)creatureData.posX, (int)creatureData.posY);
          Creature creat = Managers.Object.Spawn<Creature>(pos, creatureData.dataID, "Warrior");
          Managers.Map.MoveTo(creat, pos, true);

          for (int i = 0; i < creatureData.jobPriority.Count; i++)
            creat.SetOrAddJobPriority((FJob)i, creatureData.jobPriority[i], true);

          for (int i = 0; i < creatureData.pJobPriority.Count; i++)
            creat.SetOrAddJobPriority((FPersonalJob)i, creatureData.pJobPriority[i], true);

          break;
      }
    }
  }
}
