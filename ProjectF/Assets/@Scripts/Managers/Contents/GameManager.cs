using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BackEnd;
using static Define;
using System.Linq;
using System.Text;

#region SaveData

public class GameSaveData
{
  public RealGameData realGameData = new RealGameData();

  public List<CreatureSaveData> creatureSaveData = new List<CreatureSaveData> ();
  public List<EnvSaveData> envSaveData = new List<EnvSaveData> ();
  public StructSaveData structSaveData = new StructSaveData();
  public List<ItemHoldersSaveData> itemHolderSaveData = new List<ItemHoldersSaveData> ();
  public List<BuildObjectSaveData> buildObjectSaveData = new List<BuildObjectSaveData> ();
  public List<ItemSaveData> itemSaveData = new List<ItemSaveData>();
}

public class RealGameData
{
  public int baseLevel;
  public Dictionary<int, int> researchDic = new Dictionary<int, int>();
}

public class CreatureSaveData
{
  public int dataID;
  public string name;
  public int type;
  public float posX;
  public float posY;

  public List<float> jobPriority = Enumerable.Repeat(0f, Enum.GetValues(typeof(FJob)).Length).ToList();
  public List<float> pJobPriority = Enumerable.Repeat(0f, Enum.GetValues(typeof(FPersonalJob)).Length).ToList();
}

public class EnvSaveData
{
  public int dataID;
  public string name;
  public int type;
  public float posX;
  public float posY;
}

public class StructSaveData
{
  public List<ToiletSaveData> toiletSaveData = new List<ToiletSaveData> ();
  public List<EatingTableSaveData> eatingTableSaveData = new List<EatingTableSaveData> ();
  public List<StorageSaveData> storageSaveData = new List<StorageSaveData> ();
  public List<BedSaveData> bedSaveData = new List<BedSaveData> ();
  public List<StationSaveData> stationSaveData = new List<StationSaveData>();
  public List<BuildObjectFSaveData> buildObjectFSaveData = new List<BuildObjectFSaveData> ();
  public List<PlowBowlSaveData> plowBowlSaveData = new List<PlowBowlSaveData> ();
  public List<PlowSoilSaveData> plowSoilSaveData = new List<PlowSoilSaveData> ();
  public List<FactorySaveData> factorySaveData = new List<FactorySaveData> ();
}

#region StructSaveDataBySubType
public class ToiletSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

public class EatingTableSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

public class StorageSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;

  public List<StorageItem> storageItem = new List<StorageItem>();
}

public class BedSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

public class StationSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

public class BuildObjectFSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

public class PlowBowlSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

public class PlowSoilSaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

public class FactorySaveData
{
  public int dataID;
  public string name;
  public int type;
  public int subType;
  public float posX;
  public float posY;
}

#endregion

public class ItemHoldersSaveData
{
  public int dataID;
  public float posX;
  public float posY;
  public float mass;
  public int stack;
  public bool isDropped;
}

public class BuildObjectSaveData
{
  public int dataID;
  public float posX;
  public float posY;
}

public class ItemSaveData
{
  public int dataID;
  public float mass;
}
#endregion
public class GameManager
{
  #region GameData
  GameSaveData _saveData = new GameSaveData ();
  public GameSaveData SaveData { get { return _saveData; } set { _saveData = value; } }
  public bool LoadFlag = false;

  private string gameDataRowInData = string.Empty;

  public void GameDataInsert()
  {
    Param param = new Param();

    RealGameDataInsert(param);
    CreatureDataInsert(param);
    EnvDataInsert(param);
    StructDataInsert(param);
    ItemHolderDataInsert(param);
    ItemSaveDataInsert(param);

    Debug.Log("게임 데이터 삽입 요청합니다.");
    var bro = Backend.GameData.Insert("TEST_DATA", param);

    if (bro.IsSuccess())
    {
      Debug.Log("게임 데이터 삽입 성공." + bro);

      gameDataRowInData = bro.GetInDate();
    }
    else
    {
      Debug.LogError("게임 데이터 삽입 실패" + bro);
    }
  }

  public bool GameDataGet()
  {
    Debug.Log("게임 데이터를 불러옵니다");

    var bro = Backend.GameData.GetMyData("TEST_DATA", new Where());

    if(bro.IsSuccess())
    {
      Debug.Log("게임 데이터 조회에 성공했습니다." + bro);
      LitJson.JsonData gameDataJson = bro.FlattenRows();
      if(gameDataJson.Count <= 0)
      {
        Debug.Log("저장된 게임데이터가 없습니다.");
        return false;
      }
      else
      {
        gameDataRowInData = gameDataJson[0]["inDate"].ToString();

        RealGameDataGet(gameDataJson);
        CreatureDataGet(gameDataJson);
        EnvDataGet(gameDataJson);
        StructureDataGet(gameDataJson);
        ItemHolderDataGet(gameDataJson);
        ItemDataGet(gameDataJson);

        return true;
      }
    }
    else
    {
      Debug.Log("게임 데이터 조회에 실패했습니다." + bro);
      return false;
    }
  }

  public void GameDataUpdate()
  {
    Param param = new Param();

    RealGameDataUpdate(param);
    CreatureDataUpdate(param);
    EnvDataUpdate(param);
    StructureDataUpdate(param);
    ItemHolderDataUpdate(param);
    ItemDataUpdate(param);

    BackendReturnObject bro = null;
    if (string.IsNullOrEmpty(gameDataRowInData))
    {
      Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");
      bro = Backend.GameData.Update("TEST_DATA", new Where(), param);
    }
    else
    {
      Debug.Log($"{gameDataRowInData} 의 게임 정보 데이터 수정을 요청합니다.");
      bro = Backend.GameData.UpdateV2("TEST_DATA", gameDataRowInData, Backend.UserInDate, param);
    }

    if (bro.IsSuccess())
    {
      Debug.Log("데이터의 수정을 완료했습니다." + bro);
    }
    else
    {
      Debug.LogError("데이터의 수정을 실패했습니다." + bro);
    }
  }
  #region DataHelpers

  #region DataUpdate
  public void RealGameDataUpdate(Param param)
  {
    param.Add("rgsavedata", SaveData.realGameData);
  }

  public void CreatureDataUpdate(Param param)
  {
    var creatures = Managers.Object.Creatures;
    if (SaveData.creatureSaveData.Count > 0) SaveData.creatureSaveData.Clear();

    foreach (var creature in creatures)
    {
      CreatureSaveData creatureSaveData = new CreatureSaveData();
      creatureSaveData.dataID = creature.dataTemplateID;
      creatureSaveData.name = creature.name;
      creatureSaveData.type = (int)creature.CreatureType;
      creatureSaveData.posX = creature.transform.position.x;
      creatureSaveData.posY = creature.transform.position.y;

      foreach (var priority in creature.JobDic)
      {
        creatureSaveData.jobPriority[(int)priority.Key] = priority.Value.Priority;
      }
      foreach (var priority in creature.PersonalDic)
      {
        creatureSaveData.pJobPriority[(int)priority.Key] = priority.Value.Priority;
      }

      SaveData.creatureSaveData.Add(creatureSaveData);
    }

    param.Add("csavedata", SaveData.creatureSaveData);

  }

  public void EnvDataUpdate(Param param)
  {
    var envs = Managers.Object.Envs;
    if (SaveData.envSaveData.Count > 0) SaveData.envSaveData.Clear();

    foreach (var env in envs)
    {
      EnvSaveData envSaveData = new EnvSaveData();
      envSaveData.dataID = env.dataTemplateID;
      envSaveData.name = env.Name;
      envSaveData.type = (int)env.EnvType;
      envSaveData.posX = env.transform.position.x;
      envSaveData.posY = env.transform.position.y;

      SaveData.envSaveData.Add(envSaveData);
    }

    param.Add("esavedata", SaveData.envSaveData);

  }

  public void StructureDataUpdate(Param param)
  {
    var structures = Managers.Object.Structures;
    if (SaveData.structSaveData.toiletSaveData.Count > 0) SaveData.structSaveData.toiletSaveData.Clear();
    if (SaveData.structSaveData.eatingTableSaveData.Count > 0) SaveData.structSaveData.eatingTableSaveData.Clear();
    if (SaveData.structSaveData.storageSaveData.Count > 0) SaveData.structSaveData.storageSaveData.Clear();
    if (SaveData.structSaveData.bedSaveData.Count > 0) SaveData.structSaveData.bedSaveData.Clear();
    if (SaveData.structSaveData.stationSaveData.Count > 0) SaveData.structSaveData.stationSaveData.Clear();
    if (SaveData.structSaveData.plowBowlSaveData.Count > 0) SaveData.structSaveData.plowBowlSaveData.Clear();
    if (SaveData.structSaveData.buildObjectFSaveData.Count > 0) SaveData.structSaveData.buildObjectFSaveData.Clear();
    if (SaveData.structSaveData.plowSoilSaveData.Count > 0) SaveData.structSaveData.plowSoilSaveData.Clear();

    foreach (var structure in structures)
    {
      switch(structure.StructureSubType)
      {
        case FStructureSubType.Toilet:
          ToiletSaveData toilet = new ToiletSaveData();
          toilet.dataID = structure.dataTemplateID;
          toilet.name = structure.Name;
          toilet.type = (int)structure.StructureType;
          toilet.subType = (int)structure.StructureSubType;
          toilet.posX = structure.transform.position.x;
          toilet.posY = structure.transform.position.y;

          SaveData.structSaveData.toiletSaveData.Add(toilet);
          break;
        case FStructureSubType.EatingTable:
          EatingTableSaveData eatingTable = new EatingTableSaveData();
          eatingTable.dataID = structure.dataTemplateID;
          eatingTable.name = structure.Name;
          eatingTable.type = (int)structure.StructureType;
          eatingTable.subType = (int)structure.StructureSubType;
          eatingTable.posX = structure.transform.position.x;  
          eatingTable.posY = structure.transform.position.y;

          SaveData.structSaveData.eatingTableSaveData.Add(eatingTable);
          break;
        case FStructureSubType.Storage:
          StorageSaveData storage = new StorageSaveData();
          storage.dataID = structure.dataTemplateID;
          storage.name = structure.Name;
          storage.type = (int)structure.StructureType;
          storage.subType = (int)structure.StructureSubType;
          storage.posX = structure.transform.position.x;
          storage.posY = structure.transform.position.y;
          var store = structure as Storage;
          foreach (var s in store.storageItem)
            storage.storageItem.Add(new StorageItem(s.id, s.mass, s.label));

          SaveData.structSaveData.storageSaveData.Add(storage);
          break;
        case FStructureSubType.Bed:
          BedSaveData bed = new BedSaveData();
          bed.dataID = structure.dataTemplateID;
          bed.name = structure.Name;
          bed.type = (int)structure.StructureType;
          bed.subType = (int)structure.StructureSubType;
          bed.posX = structure.transform.position.x;
          bed.posY = structure.transform.position.y;

          SaveData.structSaveData.bedSaveData.Add(bed);
          break;
        case FStructureSubType.Station:
          StationSaveData station = new StationSaveData();
          station.dataID = structure.dataTemplateID;
          station.name = structure.Name;
          station.type = (int)structure.StructureType;
          station.subType = (int)structure.StructureSubType;
          station.posX = structure.transform.position.x;
          station.posY = structure.transform.position.y;

          SaveData.structSaveData.stationSaveData.Add(station);
          break;
        case FStructureSubType.BuildObject:
          BuildObjectFSaveData buildObject = new BuildObjectFSaveData();
          buildObject.dataID = structure.dataTemplateID;
          buildObject.name = structure.Name;
          buildObject.type = (int)structure.StructureType;
          buildObject.subType = (int)structure.StructureSubType;
          buildObject.posX = structure.transform.position.x;
          buildObject.posY = structure.transform.position.y;

          SaveData.structSaveData.buildObjectFSaveData.Add(buildObject);
          break;
        case FStructureSubType.PlowBowl:
          PlowBowlSaveData plowBowl = new PlowBowlSaveData();
          plowBowl.dataID = structure.dataTemplateID;
          plowBowl.name = structure.Name;
          plowBowl.type = (int)structure.StructureType;
          plowBowl.subType = (int)structure.StructureSubType;
          plowBowl.posX = structure.transform.position.x;
          plowBowl.posY = structure.transform.position.y;

          SaveData.structSaveData.plowBowlSaveData.Add(plowBowl);
          break;
        case FStructureSubType.Soil:
          PlowSoilSaveData plowSoil = new PlowSoilSaveData();
          plowSoil.dataID = structure.dataTemplateID;
          plowSoil.name = structure.Name;
          plowSoil.type = (int)structure.StructureType;
          plowSoil.subType = (int)structure.StructureSubType;
          plowSoil.posX = structure.transform.position.x;
          plowSoil.posY = structure.transform.position.y;

          SaveData.structSaveData.plowSoilSaveData.Add(plowSoil);
          break;
        case FStructureSubType.Factory:
          FactorySaveData factory = new FactorySaveData();
          factory.dataID = structure.dataTemplateID;
          factory.name = structure.Name;
          factory.type = (int)structure.StructureType;
          factory.subType = (int)structure.StructureSubType;
          factory.posX = structure.transform.position.x;
          factory.posY = structure.transform.position.y;

          SaveData.structSaveData.factorySaveData.Add(factory);
          break;
      }
    }

    param.Add("ssavedata", SaveData.structSaveData);
  }

  public void ItemHolderDataUpdate(Param param)
  {
    var itemHolders = Managers.Object.ItemHolders;
    if (SaveData.itemHolderSaveData.Count > 0) SaveData.itemHolderSaveData.Clear();

    foreach (var itemHolder in itemHolders)
    {
      if (!itemHolder.isDropped) continue;
      ItemHoldersSaveData itemholderSaveData = new ItemHoldersSaveData();
      itemholderSaveData.dataID = itemHolder.dataTemplateID;
      itemholderSaveData.posX = itemHolder.transform.position.x;
      itemholderSaveData.posY = itemHolder.transform.position.y;
      itemholderSaveData.mass = itemHolder.mass;
      itemholderSaveData.stack = itemHolder.stack;
      itemholderSaveData.isDropped = itemHolder.isDropped;

      SaveData.itemHolderSaveData.Add(itemholderSaveData);
    }

    param.Add("ihsavedata", SaveData.itemHolderSaveData);
  }

  public void ItemDataUpdate(Param param)
  {
    var items = Managers.Object.ItemStorage;
    if (SaveData.itemSaveData.Count > 0) SaveData.itemSaveData.Clear();

    foreach (var item in items)
    {
      ItemSaveData itemSaveData = new ItemSaveData();
      itemSaveData.dataID = item.Key;
      itemSaveData.mass = item.Value.mass;

      SaveData.itemSaveData.Add(itemSaveData);
    }

    param.Add("isavedata", SaveData.itemSaveData);
  }

  #endregion

  #region DataInsert

  public void RealGameDataInsert(Param param)
  {
    SaveData.realGameData.baseLevel = 0;
    param.Add("rgsavedata", SaveData.realGameData);
  }

  public void CreatureDataInsert(Param param)
  {
    var creatures = Managers.Object.Creatures;

    foreach (var creature in creatures)
    {
      CreatureSaveData creatureSaveData = new CreatureSaveData();
      creatureSaveData.dataID = creature.dataTemplateID;
      creatureSaveData.name = creature.name;
      creatureSaveData.type = (int)creature.CreatureType;
      creatureSaveData.posX = creature.transform.position.x;
      creatureSaveData.posY = creature.transform.position.y;

      foreach (var priority in creature.JobDic)
      {
        creatureSaveData.jobPriority[(int)priority.Key] = priority.Value.Priority;
      }
      foreach (var priority in creature.PersonalDic)
      {
        creatureSaveData.pJobPriority[(int)priority.Key] = priority.Value.Priority;
      }

      SaveData.creatureSaveData.Add(creatureSaveData);
    }

    Debug.Log("DB 업데이트 목록에 해당 데이터들을 추가합니다.");
    param.Add("csavedata", SaveData.creatureSaveData);
    //param.Add("dataID", creatureSaveData.dataID);
    //param.Add("name", creatureSaveData.name);
    //param.Add("posX", creatureSaveData.posX);
    //param.Add("posY", creatureSaveData.posY);

    //param.Add("jobPriority", creatureSaveData.jobPriority);
    //param.Add("pJobPriority", creatureSaveData.pJobPriority);


    
  }

  public void EnvDataInsert(Param param)
  {
    var envs = Managers.Object.Envs;

    foreach (var env in envs)
    {
      EnvSaveData envSaveData = new EnvSaveData();
      envSaveData.dataID = env.dataTemplateID;
      envSaveData.name = env.Name;
      envSaveData.type = (int)env.EnvType;
      envSaveData.posX = env.transform.position.x;
      envSaveData.posY = env.transform.position.y;

      SaveData.envSaveData.Add(envSaveData);
    }
    param.Add("esavedata", SaveData.envSaveData);
  }

  public void StructDataInsert(Param param)
  {
    var structures = Managers.Object.Structures;

    foreach (var structure in structures)
    {
      switch(structure.StructureSubType)
      {
        case FStructureSubType.Toilet:
          ToiletSaveData toilet = new ToiletSaveData();
          toilet.dataID = structure.dataTemplateID;
          toilet.name = structure.Name;
          toilet.type = (int)structure.StructureType;
          toilet.subType = (int)structure.StructureSubType;
          toilet.posX = structure.transform.position.x;
          toilet.posY = structure.transform.position.y;

          SaveData.structSaveData.toiletSaveData.Add(toilet);
          break;
        case FStructureSubType.EatingTable:
          EatingTableSaveData eatingTable = new EatingTableSaveData();
          eatingTable.dataID = structure.dataTemplateID;
          eatingTable.name = structure.Name;
          eatingTable.type = (int)structure.StructureType;
          eatingTable.subType = (int)structure.StructureSubType;
          eatingTable.posX = structure.transform.position.x;
          eatingTable.posY = structure.transform.position.y;

          SaveData.structSaveData.eatingTableSaveData.Add(eatingTable);
          break;
        case FStructureSubType.Storage:
          StorageSaveData storage = new StorageSaveData();
          storage.dataID = structure.dataTemplateID;
          storage.name = structure.Name;
          storage.type = (int)structure.StructureType;
          storage.subType = (int)structure.StructureSubType;
          storage.posX = structure.transform.position.x;
          storage.posY = structure.transform.position.y;
          var store = structure as Storage;
          foreach (var s in store.storageItem)
            storage.storageItem.Add(new StorageItem(s.id, s.mass, s.label));

          SaveData.structSaveData.storageSaveData.Add(storage);
          break;
        case FStructureSubType.Bed:
          BedSaveData bed = new BedSaveData();
          bed.dataID = structure.dataTemplateID;
          bed.name = structure.Name;
          bed.type = (int)structure.StructureType;
          bed.subType = (int)structure.StructureSubType;
          bed.posX = structure.transform.position.x;
          bed.posY = structure.transform.position.y;

          SaveData.structSaveData.bedSaveData.Add(bed);
          break;
        case FStructureSubType.Station:
          StationSaveData station = new StationSaveData();
          station.dataID = structure.dataTemplateID;
          station.name = structure.Name;
          station.type = (int)structure.StructureType;
          station.subType = (int)structure.StructureSubType;
          station.posX = structure.transform.position.x;
          station.posY = structure.transform.position.y;

          SaveData.structSaveData.stationSaveData.Add(station);
          break;
        case FStructureSubType.BuildObject:
          BuildObjectFSaveData buildObject = new BuildObjectFSaveData();
          buildObject.dataID = structure.dataTemplateID;
          buildObject.name = structure.Name;
          buildObject.type = (int)structure.StructureType;
          buildObject.subType = (int)structure.StructureSubType;
          buildObject.posX = structure.transform.position.x;
          buildObject.posY = structure.transform.position.y;

          SaveData.structSaveData.buildObjectFSaveData.Add(buildObject);
          break;
        case FStructureSubType.PlowBowl:
          PlowBowlSaveData plowBowl = new PlowBowlSaveData();
          plowBowl.dataID = structure.dataTemplateID;
          plowBowl.name = structure.Name;
          plowBowl.type = (int)structure.StructureType;
          plowBowl.subType = (int)structure.StructureSubType;
          plowBowl.posX = structure.transform.position.x;
          plowBowl.posY = structure.transform.position.y;

          SaveData.structSaveData.plowBowlSaveData.Add(plowBowl);
          break;
        case FStructureSubType.Soil:
          PlowSoilSaveData plowSoil = new PlowSoilSaveData();
          plowSoil.dataID = structure.dataTemplateID;
          plowSoil.name = structure.Name;
          plowSoil.type = (int)structure.StructureType;
          plowSoil.subType = (int)structure.StructureSubType;
          plowSoil.posX = structure.transform.position.x;
          plowSoil.posY = structure.transform.position.y;

          SaveData.structSaveData.plowSoilSaveData.Add(plowSoil);
          break;
        case FStructureSubType.Factory:
          FactorySaveData factory = new FactorySaveData();
          factory.dataID = structure.dataTemplateID;
          factory.name = structure.Name;
          factory.type = (int)structure.StructureType;
          factory.subType = (int)structure.StructureSubType;
          factory.posX = structure.transform.position.x;
          factory.posY = structure.transform.position.y;

          SaveData.structSaveData.factorySaveData.Add(factory);
          break;

      }
      //StructSaveData structSaveData = new StructSaveData();
      //structSaveData.dataID = structure.dataTemplateID;
      //structSaveData.name = structure.Name;
      //structSaveData.type = (int)structure.StructureType;
      //structSaveData.subType = (int)structure.StructureSubType;
      //structSaveData.posX = structure.transform.position.x;
      //structSaveData.posY = structure.transform.position.y;

      //SaveData.structSaveData.Add(structSaveData);
    }
    param.Add("ssavedata", SaveData.structSaveData);
  }

  public void ItemHolderDataInsert(Param param)
  {
    var itemHolders = Managers.Object.ItemHolders;

    foreach (var itemHolder in itemHolders)
    {
      ItemHoldersSaveData itemHolderSaveData = new ItemHoldersSaveData();
      itemHolderSaveData.dataID = itemHolder.dataTemplateID;
      itemHolderSaveData.posX = itemHolder.transform.position.x;
      itemHolderSaveData.posY = itemHolder.transform.position.y;
      itemHolderSaveData.mass = itemHolder.mass;
      itemHolderSaveData.stack = itemHolder.stack;
      itemHolderSaveData.isDropped = itemHolder.isDropped;

      SaveData.itemHolderSaveData.Add(itemHolderSaveData);
    }
    param.Add("ihsavedata", SaveData.itemHolderSaveData);
  }

  public void ItemSaveDataInsert(Param param)
  {
    var items = Managers.Object.ItemStorage;

    foreach(var item in items)
    {
      ItemSaveData itemSaveData = new ItemSaveData();
      itemSaveData.dataID = item.Key;
      itemSaveData.mass = item.Value.mass;

      SaveData.itemSaveData.Add(itemSaveData);
    }

    param.Add("isavedata", SaveData.itemSaveData);
  }

  #endregion

  #region DataGet
  public bool RealGameDataGet(LitJson.JsonData gameDataJson)
  {
    if (gameDataJson.Count <= 0)
    {
      Debug.LogWarning("데이터가 존재하지 않습니다.");
      return false;
    }
    else
    {
      var rgData = gameDataJson[0]["rgsavedata"];

      if (rgData == null || rgData.Count <= 0)
      {
        Debug.Log("게임 내부 데이터가 존재하지 않습니다.");
        return false;
      }

      if (SaveData.realGameData is null) SaveData.realGameData = new RealGameData();

      SaveData.realGameData.baseLevel = int.Parse(rgData["baseLevel"].ToString());
      foreach(var keystring in rgData["researchDic"].Keys)
      {
        SaveData.realGameData.researchDic.Add(int.Parse(keystring), int.Parse(rgData["researchDic"][keystring].ToString()));
      }

      return true;
    }
  }

  public bool CreatureDataGet(LitJson.JsonData gameDataJson)
  {
    if (gameDataJson.Count <= 0)
    {
      Debug.LogWarning("데이터가 존재하지 않습니다.");
      return false;
    }
    else
    {
      var creatureData = gameDataJson[0]["csavedata"];

      if (creatureData == null || creatureData.Count <= 0)
      {
        Debug.Log("크리쳐의 데이터가 존재하지 않습니다.");
        return false;
      }

      if(SaveData.creatureSaveData.Count > 0) SaveData.creatureSaveData.Clear();

      foreach (LitJson.JsonData data in creatureData)
      {
        CreatureSaveData creatureLoadData = new CreatureSaveData();
        creatureLoadData.dataID = int.Parse(data["dataID"].ToString());
        creatureLoadData.name = data["name"].ToString();
        creatureLoadData.type = int.Parse(data["type"].ToString());
        creatureLoadData.posX = float.Parse(data["posX"].ToString());
        creatureLoadData.posY = float.Parse(data["posY"].ToString());

        int count = 0;
        foreach (LitJson.JsonData Indata in data["jobPriority"])
        {
          creatureLoadData.jobPriority[count] = int.Parse(Indata.ToString());
          count++;
        }
        count = 0;
        foreach (LitJson.JsonData Indata in data["pJobPriority"])
        {
          creatureLoadData.pJobPriority[count] = int.Parse(Indata.ToString());
          count++;
        }

        SaveData.creatureSaveData.Add(creatureLoadData);
      }

      return true;
    }
  }

  public bool EnvDataGet(LitJson.JsonData gameDataJson)
  {
    if (gameDataJson.Count <= 0)
    {
      Debug.LogWarning("데이터가 존재하지 않습니다.");
      return false;
    }
    else
    {
      var envData = gameDataJson[0]["esavedata"];

      if (envData == null || envData.Count <= 0)
      {
        Debug.Log("환경요소의 데이터가 존재하지 않습니다.");
        return false;
      }

      if (SaveData.envSaveData.Count > 0) SaveData.envSaveData.Clear();

      foreach (LitJson.JsonData data in envData)
      {
        EnvSaveData envLoadData = new EnvSaveData();
        envLoadData.dataID = int.Parse(data["dataID"].ToString());
        envLoadData.name = data["name"].ToString();
        envLoadData.type = int.Parse(data["type"].ToString());
        envLoadData.posX = float.Parse(data["posX"].ToString());
        envLoadData.posY = float.Parse(data["posY"].ToString());

        SaveData.envSaveData.Add(envLoadData);
      }

      return true;
    }
  }

  public bool StructureDataGet(LitJson.JsonData gameDataJson)
  {
    if (gameDataJson.Count <= 0)
    {
      Debug.LogWarning("데이터가 존재하지 않습니다.");
      return false;
    }
    else
    {
      var structureData = gameDataJson[0]["ssavedata"];

      if (structureData == null || structureData.Count <= 0)
      {
        Debug.Log("구조물의 데이터가 존재하지 않습니다.");
        return false;
      }

      //if (SaveData.structSaveData.Count > 0) SaveData.structSaveData.Clear();

      foreach(LitJson.JsonData data in structureData["toiletSaveData"])
      {
        ToiletSaveData toilet = new ToiletSaveData();
        toilet.dataID = int.Parse(data["dataID"].ToString());
        toilet.name = data["name"].ToString();
        toilet.type = int.Parse(data["type"].ToString());
        toilet.posX = float.Parse(data["posX"].ToString());
        toilet.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.toiletSaveData.Add(toilet);
      }

      foreach (LitJson.JsonData data in structureData["eatingTableSaveData"])
      {
        EatingTableSaveData eatingTable = new EatingTableSaveData();
        eatingTable.dataID = int.Parse(data["dataID"].ToString());
        eatingTable.name = data["name"].ToString();
        eatingTable.type = int.Parse(data["type"].ToString());
        eatingTable.posX = float.Parse(data["posX"].ToString());
        eatingTable.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.eatingTableSaveData.Add(eatingTable);
      }

      foreach (LitJson.JsonData data in structureData["storageSaveData"])
      {
        StorageSaveData storage = new StorageSaveData();
        storage.dataID = int.Parse(data["dataID"].ToString());
        storage.name = data["name"].ToString();
        storage.type = int.Parse(data["type"].ToString());
        storage.posX = float.Parse(data["posX"].ToString());
        storage.posY = float.Parse(data["posY"].ToString());
        foreach (LitJson.JsonData Indata in data["storageItem"])
        {
          storage.storageItem.Add(new StorageItem(int.Parse(Indata["id"].ToString()), float.Parse(Indata["mass"].ToString()), Indata["label"].ToString()));
        }

        SaveData.structSaveData.storageSaveData.Add(storage);
      }

      foreach (LitJson.JsonData data in structureData["bedSaveData"])
      {
        BedSaveData bed = new BedSaveData();
        bed.dataID = int.Parse(data["dataID"].ToString());
        bed.name = data["name"].ToString();
        bed.type = int.Parse(data["type"].ToString());
        bed.posX = float.Parse(data["posX"].ToString());
        bed.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.bedSaveData.Add(bed);
      }

      foreach (LitJson.JsonData data in structureData["stationSaveData"])
      {
        StationSaveData station = new StationSaveData();
        station.dataID = int.Parse(data["dataID"].ToString());
        station.name = data["name"].ToString();
        station.type = int.Parse(data["type"].ToString());
        station.posX = float.Parse(data["posX"].ToString());
        station.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.stationSaveData.Add(station);
      }

      foreach (LitJson.JsonData data in structureData["buildObjectFSaveData"])
      {
        BuildObjectFSaveData buildObject = new BuildObjectFSaveData();
        buildObject.dataID = int.Parse(data["dataID"].ToString());
        buildObject.name = data["name"].ToString();
        buildObject.type = int.Parse(data["type"].ToString());
        buildObject.posX = float.Parse(data["posX"].ToString());
        buildObject.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.buildObjectFSaveData.Add(buildObject);
      }

      foreach (LitJson.JsonData data in structureData["plowBowlSaveData"])
      {
        PlowBowlSaveData plowbowl = new PlowBowlSaveData();
        plowbowl.dataID = int.Parse(data["dataID"].ToString());
        plowbowl.name = data["name"].ToString();
        plowbowl.type = int.Parse(data["type"].ToString());
        plowbowl.posX = float.Parse(data["posX"].ToString());
        plowbowl.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.plowBowlSaveData.Add(plowbowl);
      }

      foreach (LitJson.JsonData data in structureData["plowSoilSaveData"])
      {
        PlowSoilSaveData plowsoil = new PlowSoilSaveData();
        plowsoil.dataID = int.Parse(data["dataID"].ToString());
        plowsoil.name = data["name"].ToString();
        plowsoil.type = int.Parse(data["type"].ToString());
        plowsoil.posX = float.Parse(data["posX"].ToString());
        plowsoil.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.plowSoilSaveData.Add(plowsoil);
      }

      foreach (LitJson.JsonData data in structureData["factorySaveData"])
      {
        FactorySaveData factory = new FactorySaveData();
        factory.dataID = int.Parse(data["dataID"].ToString());
        factory.name = data["name"].ToString();
        factory.type = int.Parse(data["type"].ToString());
        factory.posX = float.Parse(data["posX"].ToString());
        factory.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.factorySaveData.Add(factory);
      }



      //foreach (LitJson.JsonData data in structureData)
      //{
      //  StructSaveData structureLoadData = new StructSaveData();

      //  structureLoadData.dataID = int.Parse(data["dataID"].ToString());
      //  structureLoadData.name = data["name"].ToString();
      //  structureLoadData.type = int.Parse(data["type"].ToString());
      //  structureLoadData.posX = float.Parse(data["posX"].ToString());
      //  structureLoadData.posY = float.Parse(data["posY"].ToString());

      //  SaveData.structSaveData.Add(structureLoadData);
      //}

      return true;
    }
  }

  public bool ItemHolderDataGet(LitJson.JsonData gameDataJson)
  {
    if (gameDataJson.Count <= 0)
    {
      Debug.LogWarning("데이터가 존재하지 않습니다.");
      return false;
    }
    else
    {
      var itemholderData = gameDataJson[0]["ihsavedata"];

      if (itemholderData == null || itemholderData.Count <= 0)
      {
        Debug.Log("아이템 홀더의 데이터가 존재하지 않습니다.");
        return false;
      }

      if (SaveData.itemHolderSaveData.Count > 0) SaveData.itemHolderSaveData.Clear();

      foreach (LitJson.JsonData data in itemholderData)
      {
        ItemHoldersSaveData itemholderLoadData = new ItemHoldersSaveData();
        itemholderLoadData.dataID = int.Parse(data["dataID"].ToString());
        itemholderLoadData.posX = float.Parse(data["posX"].ToString());
        itemholderLoadData.posY = float.Parse(data["posY"].ToString());
        itemholderLoadData.mass = float.Parse(data["mass"].ToString());
        itemholderLoadData.stack = int.Parse(data["stack"].ToString());
        itemholderLoadData.isDropped = bool.Parse(data["isDropped"].ToString());

        SaveData.itemHolderSaveData.Add(itemholderLoadData);
      }

      return true;
    }
  }

  public bool ItemDataGet(LitJson.JsonData gameDataJson)
  {
    if (gameDataJson.Count <= 0)
    {
      Debug.LogWarning("데이터가 존재하지 않습니다.");
      return false;
    }
    else
    {
      var itemData = gameDataJson[0]["isavedata"];

      if (itemData == null || itemData.Count <= 0)
      {
        Debug.Log("아이템의 데이터가 존재하지 않습니다.");
        return false;
      }

      if (SaveData.itemSaveData.Count > 0) SaveData.itemSaveData.Clear();

      foreach (LitJson.JsonData data in itemData)
      {
        ItemSaveData itemLoadData = new ItemSaveData();
        itemLoadData.dataID = int.Parse(data["dataID"].ToString());
        itemLoadData.mass = float.Parse(data["mass"].ToString());

        SaveData.itemSaveData.Add(itemLoadData);
      }

      return true;
    }
  }
  #endregion

  #endregion

  #endregion

  #region MoveDir
  private Vector2 _moveDir;
  public Vector2 MoveDir
  {
    get { return _moveDir; }
    set
    {
      _moveDir = value;
      OnMoveDirChanged?.Invoke(value);
    }
  }

  public event Action<Vector2> OnMoveDirChanged;

  public event Action<Enum, bool, bool> onJobAbleChanged;
  public void OnJobAbleChanged(Enum job, bool set, bool personal = false)
  {
    onJobAbleChanged?.Invoke(job, set, personal);
  }
  #endregion

  #region Language
  private FLanguage _language = FLanguage.Korean;
  public FLanguage Language
  {
    get { return _language; }
    set
    {
      _language = value;
    }
  }

  public string GetText(string textId, object[] args)
  {
    string formatStr = "";

    switch(_language)
    {
      case FLanguage.Korean:
        formatStr = Managers.Data.TextDic[textId].KOR;
        break;
    }

    int argIndex = 0;

    while(argIndex < args.Length)
    {
      if(args[argIndex] is string)
      {
        formatStr = formatStr.Replace("%s", args[argIndex]?.ToString() ?? "[Invalid String]");
      }
      else if (args[argIndex] is int)
      {
        formatStr = formatStr.Replace("%d", args[argIndex]?.ToString() ?? "[Invalid Int]");
      }
      else if (args[argIndex] is float || args[argIndex] is double)
      {
        formatStr = formatStr.Replace("%f", args[argIndex]?.ToString() ?? "[Invalid Float]");
      }
      //TODO
      argIndex++;
    }
    return formatStr;
  }

  public string GetText(string textId, object args)
  {
    string formatStr = "";

    switch (_language)
    {
      case FLanguage.Korean:
        formatStr = Managers.Data.TextDic[textId].KOR;
        break;
    }

    if (args is string)
    {
      formatStr = formatStr.Replace("%s", args?.ToString() ?? "[Invalid String]");
    }
    else if (args is int)
    {
      formatStr = formatStr.Replace("%d", args?.ToString() ?? "[Invalid Int]");
    }
    else if (args is float || args is double)
    {
      formatStr = formatStr.Replace("%f", args?.ToString() ?? "[Invalid Float]");
    }
    return formatStr;
  }
  #endregion

  #region Save & Load
  public string Path { get { return Application.persistentDataPath + "/SaveData.json"; } }

  public void InitGame()
  {
    Managers.Object.Spawn<Creature>(Vector3.zero, CREATURE_WARRIOR_DATAID, "Warrior");
    Managers.RandomSeedGenerate.GenerateMaps(); //랜덤 시드 생성
    GameDataInsert();

  }

  public void UpdateGame()
  {
    GameDataUpdate();
  }

  public void SaveGame()
  {

  }

  public void LoadGame()
  {
    if(GameDataGet() == true)
    {
      LoadFlag = true;
    }
   

  }



  #endregion
}
