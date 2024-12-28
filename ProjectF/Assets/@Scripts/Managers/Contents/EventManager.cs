using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EventManager
{
  //DescriptionNotice
  public Action<FObjectType, int> description;
  private GameObject DescriptionObj;
  public void ShowDescription(Vector2 localPos, Transform parent, FObjectType type, int id)
  {
    if (DescriptionObj is null)
        DescriptionObj = Managers.Resource.Instantiate("UI_DescriptionPopup", parent);
    DescriptionObj.GetComponent<RectTransform>().localPosition = localPos;
    description?.Invoke(type, id);
  } 

  //Notice
  public Action<string> notice;
  public void Notice(string str) => notice?.Invoke(str);

  //moodsystem
  public Action<Enum, FMood> moodChanged;
  public Action<Enum> moodStable;
  public void MoodChanged(Creature creature, Enum ppjob, FMood mood)
  {
    if(creature.MoodState != mood)
    {
      creature.MoodState = mood;
      moodChanged?.Invoke(ppjob, mood);
    }
    
  }

  public void MoodStable(Creature creature, Enum ppjob)
  {
    creature.MoodState = FMood.None;
    moodStable?.Invoke(ppjob);
  }
}
