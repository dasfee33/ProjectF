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
  public float posX;
  public float posY;
}

public class ItemHoldersSaveData
{
  public float posX;
  public float posY;
}

public class BuildObjectSaveData
{
  public float posX;
  public float posY;
}
#endregion
public class GameManager
{
  #region GameData
  GameSaveData _saveData = new GameSaveData ();
  public GameSaveData SaveData { get { return _saveData; } set { _saveData = value; } }

  private string gameDataRowInData = string.Empty;

  public void GameDataInsert()
  {
    CreatureDataInsert();
    EnvDataInsert();
  }

  public bool GameDataGet()
  {
    Debug.Log("게임 데이터를 불러옵니다");

    var bro = Backend.GameData.GetMyData("TEST_DATA", new Where());

    if(bro.IsSuccess())
    {
      Debug.Log("게임 데이터 조회에 성공했습니다." + bro);
      LitJson.JsonData gameDataJson = bro.FlattenRows();
      gameDataRowInData = gameDataJson[0]["inDate"].ToString();

      CreatureDataGet(gameDataJson);
      EnvDataGet(gameDataJson);

      return true;
    }
    else
    {
      Debug.Log("게임 데이터 조회에 실패했습니다." + bro);
      return false;
    }
  }

  public void GameDataUpdate()
  {
    CreatureDataUpdate();
    EnvDataUpdate();
  }
  #region DataHelpers

  #region DataUpdate
  public bool CreatureDataUpdate()
  {
    Param param = new Param();
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

    BackendReturnObject bro = null;
    if(string.IsNullOrEmpty(gameDataRowInData))
    {
      Debug.Log("내 제일 최신 게임 정보 데이터 수정을 요청합니다.");
      bro = Backend.GameData.Update("TEST_DATA", new Where(), param);
    }
    else
    {
      Debug.Log($"{gameDataRowInData} 의 게임 정보 데이터 수정을 요청합니다.");
      bro = Backend.GameData.UpdateV2("TEST_DATA", gameDataRowInData, Backend.UserInDate, param);
    }

    if(bro.IsSuccess())
    {
      Debug.Log("크리쳐 데이터의 수정을 완료했습니다." + bro);
      return true;
    }
    else
    {
      Debug.LogError("크리쳐 데이터의 수정을 실패했습니다." + bro);
      return false;
    }
  }

  public bool EnvDataUpdate()
  {
    Param param = new Param();
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
      Debug.Log("환경요소 데이터의 수정을 완료했습니다." + bro);
      return true;
    }
    else
    {
      Debug.LogError("환경요소 데이터의 수정을 실패했습니다." + bro);
      return false;
    }
  }

  #endregion

  #region DataInsert

  public void CreatureDataInsert()
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
    Param param = new Param();
    param.Add("csavedata", SaveData.creatureSaveData);
    //param.Add("dataID", creatureSaveData.dataID);
    //param.Add("name", creatureSaveData.name);
    //param.Add("posX", creatureSaveData.posX);
    //param.Add("posY", creatureSaveData.posY);

    //param.Add("jobPriority", creatureSaveData.jobPriority);
    //param.Add("pJobPriority", creatureSaveData.pJobPriority);

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

  public void EnvDataInsert()
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

    Debug.Log("DB 업데이트 목록에 해당 데이터들을 추가합니다.");
    Param param = new Param();
    param.Add("esavedata", SaveData.envSaveData);
    //param.Add("dataID", creatureSaveData.dataID);
    //param.Add("name", creatureSaveData.name);
    //param.Add("posX", creatureSaveData.posX);
    //param.Add("posY", creatureSaveData.posY);

    //param.Add("jobPriority", creatureSaveData.jobPriority);
    //param.Add("pJobPriority", creatureSaveData.pJobPriority);

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
    //if (File.Exists(Path))
    //  return;

    //Managers.Data.HeroDic => 데이터 시트에 접근
  }

  public void SaveGame()
  {
    //string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
    //File.WriteAllText(Path, jsonStr);
    //Debug.Log($"Save Game Completed : {Path}");
  }

  public bool LoadGame()
  {
    GameDataGet();
    //if (File.Exists(Path) == false)
    //  return false;

    //string fileStr = File.ReadAllText(Path);
    //GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

    //if (data != null)
    //  Managers.Game.SaveData = data;

    //Debug.Log($"Save Game Loaded : {Path}");
    //return true;

    //TEMP
    return true;
  }



  #endregion
}
