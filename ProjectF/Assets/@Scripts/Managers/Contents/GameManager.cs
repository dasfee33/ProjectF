using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BackEnd;
using static Define;
using System.Linq;

#region SaveData

public class GameSaveData
{
  public List<CreatureSaveData> creatureSaveData = new List<CreatureSaveData> ();
  public List<EnvSaveData> envSaveData = new List<EnvSaveData> ();
  public List<StructSaveData> structSaveData = new List<StructSaveData> ();
  public List<ItemHoldersSaveData> itemHolderSaveData = new List<ItemHoldersSaveData> ();
  public List<BuildObjectSaveData> buildObjectSaveData = new List<BuildObjectSaveData> ();
  public List<ItemSaveData> itemSaveData = new List<ItemSaveData>();
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
  public int dataID;
  public string name;
  public int type;
  public float posX;
  public float posY;
}

public class ItemHoldersSaveData
{
  public int dataID;
  public float posX;
  public float posY;
  public float mass;
  public int stack;
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

    CreatureDataInsert(param);
    EnvDataInsert(param);
    StructDataInsert(param);
    ItemHolderDataInsert(param);

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

        CreatureDataGet(gameDataJson);
        EnvDataGet(gameDataJson);
        StructureDataGet(gameDataJson);
        ItemHolderDataGet(gameDataJson);

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

    CreatureDataUpdate(param);
    EnvDataUpdate(param);
    StructureDataUpdate(param);
    ItemHolderDataUpdate(param);

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
        creatureSaveData.pJobPriority[(int)priority.Key] = priority.Value;
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
    if (SaveData.structSaveData.Count > 0) SaveData.structSaveData.Clear();

    foreach (var structure in structures)
    {
      StructSaveData structureSaveData = new StructSaveData();
      structureSaveData.dataID = structure.dataTemplateID;
      structureSaveData.name = structure.Name;
      structureSaveData.type = (int)structure.StructureType;
      structureSaveData.posX = structure.transform.position.x;
      structureSaveData.posY = structure.transform.position.y;

      SaveData.structSaveData.Add(structureSaveData);
    }

    param.Add("ssavedata", SaveData.structSaveData);
  }

  public void ItemHolderDataUpdate(Param param)
  {
    var itemHolders = Managers.Object.ItemHolders;
    if (SaveData.itemHolderSaveData.Count > 0) SaveData.itemHolderSaveData.Clear();

    foreach (var itemHolder in itemHolders)
    {
      ItemHoldersSaveData itemholderSaveData = new ItemHoldersSaveData();
      itemholderSaveData.dataID = itemHolder.dataTemplateID;
      itemholderSaveData.posX = itemHolder.transform.position.x;
      itemholderSaveData.posY = itemHolder.transform.position.y;
      itemholderSaveData.mass = itemHolder.mass;
      itemholderSaveData.stack = itemHolder.stack;

      SaveData.itemHolderSaveData.Add(itemholderSaveData);
    }

    param.Add("ihsavedata", SaveData.itemHolderSaveData);
  } 

  #endregion

  #region DataInsert

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
        creatureSaveData.pJobPriority[(int)priority.Key] = priority.Value;
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
      StructSaveData structSaveData = new StructSaveData();
      structSaveData.dataID = structure.dataTemplateID;
      structSaveData.name = structure.Name;
      structSaveData.type = (int)structure.StructureType;
      structSaveData.posX = structure.transform.position.x;
      structSaveData.posY = structure.transform.position.y;

      SaveData.structSaveData.Add(structSaveData);
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

      SaveData.itemHolderSaveData.Add(itemHolderSaveData);
    }
    param.Add("ihsavedata", SaveData.itemHolderSaveData);
  }

  #endregion

  #region DataGet
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

      if (SaveData.structSaveData.Count > 0) SaveData.structSaveData.Clear();

      foreach (LitJson.JsonData data in structureData)
      {
        StructSaveData structureLoadData = new StructSaveData();
        structureLoadData.dataID = int.Parse(data["dataID"].ToString());
        structureLoadData.name = data["name"].ToString();
        structureLoadData.type = int.Parse(data["type"].ToString());
        structureLoadData.posX = float.Parse(data["posX"].ToString());
        structureLoadData.posY = float.Parse(data["posY"].ToString());

        SaveData.structSaveData.Add(structureLoadData);
      }

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

        SaveData.itemHolderSaveData.Add(itemholderLoadData);
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

  public event Action<Enum, bool> onJobAbleChanged;
  public void OnJobAbleChanged(Enum job, bool set)
  {
    onJobAbleChanged?.Invoke(job, set);
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

  public string GetText(string textId)
  {
    switch(_language)
    {
      case FLanguage.Korean:
        return Managers.Data.TextDic[textId].KOR;

    }
    return "";
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
