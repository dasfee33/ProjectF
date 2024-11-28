using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BackEnd;
using static Define;

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
  public float posX;
  public float posY;

  public List<float> jobPriority = new List<float>();
  public List<float> pJobPriority = new List<float>();

  
}

public class EnvSaveData
{
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
    var creatures = Managers.Object.Creatures;
    List<CreatureSaveData> csavedata = new List<CreatureSaveData>();

    foreach(var creature in creatures)
    {
      CreatureSaveData creatureSaveData = new CreatureSaveData ();
      creatureSaveData.dataID = creature.dataTemplateID;
      creatureSaveData.name = creature.name;
      creatureSaveData.posX = creature.transform.position.x;
      creatureSaveData.posY = creature.transform.position.y;

      foreach(var priority in creature.JobDic)
      {
        creatureSaveData.jobPriority.Add(priority.Value.Priority);
      }
      foreach(var priority in creature.PersonalDic)
      {
        creatureSaveData.pJobPriority.Add(priority.Value);
      }

      csavedata.Add(creatureSaveData);
    }

    Debug.Log("DB 업데이트 목록에 해당 데이터들을 추가합니다.");
    Param param = new Param();
    param.Add("csavedata", csavedata);
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

  public void GameDataGet()
  {
    Debug.Log("게임 데이터를 불러옵니다");

    var bro = Backend.GameData.GetMyData("TEST_DATA", new Where());

    if(bro.IsSuccess())
    {
      Debug.Log("게임 데이터 조회에 성공했습니다." + bro);

      LitJson.JsonData gameDataJson = bro.FlattenRows();

      if(gameDataJson.Count <= 0)
      {
        Debug.LogWarning("데이터가 존재하지 않습니다.");
      }
      else
      {
        gameDataRowInData = gameDataJson[0]["inData"].ToString();


      }
    }
    else
    {
      Debug.Log("게임 데이터 조회에 실패했습니다." + bro);
    }
  }

  public void GameDataUpdate()
  {

  }
  #region DataHelpers

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
    if (File.Exists(Path))
      return;

    //Managers.Data.HeroDic => 데이터 시트에 접근
  }

  public void SaveGame()
  {
    string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
    File.WriteAllText(Path, jsonStr);
    Debug.Log($"Save Game Completed : {Path}");
  }

  public bool LoadGame()
  {
    if (File.Exists(Path) == false)
      return false;

    string fileStr = File.ReadAllText(Path);
    GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

    if (data != null)
      Managers.Game.SaveData = data;

    Debug.Log($"Save Game Loaded : {Path}");
    return true;
  }



  #endregion
}
