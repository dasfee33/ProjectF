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
}
