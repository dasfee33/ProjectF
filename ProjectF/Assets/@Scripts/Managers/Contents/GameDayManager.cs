using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class GameDayManager
{
  public FCurrentTime currentTime;

  //TEMP
  private int period = 0;
  public int Period { get { return period; } }

  public float day = 300;
  private float hour = 0;

  private WaitForSeconds oneSec = new WaitForSeconds(1f);

  public event Action timeChanged;
  public event Action dayChanged;
  private void GameTimeChanged() => timeChanged?.Invoke();
  private void GameDayChanged() => dayChanged?.Invoke();

  public IEnumerator coDay()
  {
    while(true)
    {
      if(hour > day * 0.9f) currentTime = FCurrentTime.Dawn;
      else if(hour > day * 0.7f) currentTime = FCurrentTime.Night;
      else if(hour > day * 0.55f) currentTime = FCurrentTime.BeforeSunset;
      else currentTime = FCurrentTime.Day;

      if(hour >= day)
      {
        period++;
        GameDayChanged();
        hour = 0;
      }
      yield return oneSec;

      hour++;
      GameTimeChanged();
    }
  }
}
