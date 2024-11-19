using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

#region SaveData
[Serializable]
public class GameSaveData
{
  public List<CreatureSaveData> cretureSaveData = new List<CreatureSaveData> ();
  public List<EnvSaveData> envSaveData = new List<EnvSaveData> ();
  public List<StructSaveData> structSaveData = new List<StructSaveData> ();
  public List<ItemHoldersSaveData> itemHolderSaveData = new List<ItemHoldersSaveData> ();
  public List<BuildObjectSaveData> buildObjectSaveData = new List<BuildObjectSaveData> ();
}

[Serializable]
public class CreatureSaveData
{
  public string name;
  public float posX;
  public float posY;
}

[Serializable]
public class EnvSaveData
{
  public float posX;
  public float posY;
}

[Serializable]
public class StructSaveData
{
  public float posX;
  public float posY;
}

[Serializable]
public class ItemHoldersSaveData
{
  public float posX;
  public float posY;
}

[Serializable]
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

  #endregion

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
