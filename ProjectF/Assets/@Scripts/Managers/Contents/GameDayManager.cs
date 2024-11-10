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

  private float lerpDuration = 3f;
  private float lerpTime = 0f;
  private Color targetColor;

  public IEnumerator coDay()
  {
    while(true)
    {
      Color previous = targetColor;

      if (hour > day * 0.9f)
      {
        currentTime = FCurrentTime.Dawn;
        targetColor = COLOR.BEFORESUNSET;
      }
      else if (hour > day * 0.7f)
      {
        currentTime = FCurrentTime.Night;
        targetColor = COLOR.NIGHT;
      }
      else if (hour > day * 0.55f)
      {
        currentTime = FCurrentTime.BeforeSunset;
        targetColor = COLOR.BEFORESUNSET;
      }
      else
      {
        currentTime = FCurrentTime.Day;
        targetColor = COLOR.DAY;
      }

      if (previous != targetColor)
        lerpTime = 0f;

      if (lerpTime < lerpDuration)
      {
        Managers.GameDayLight.color = Color.Lerp(Managers.GameDayLight.color, targetColor, lerpTime / lerpDuration);
        lerpTime += Time.deltaTime;
      }

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
