using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
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
}
