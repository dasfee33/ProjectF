using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MoodSystem : InitBase
{
  public Creature Owner { get; set; }
  public SpriteRenderer CreatureBallon;
  public SpriteRenderer InfoImage;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    Owner = this.GetComponent<Creature>();

    Managers.Event.moodChanged -= InfoSet;
    Managers.Event.moodChanged += InfoSet;

    Managers.Event.moodStable -= InfoReset;
    Managers.Event.moodStable += InfoReset;

    return true;
  }

  public Enum eventJob;
  public FMood eventMood;

  public void InfoSet(Enum job, FMood mood)
  {
    if (!CreatureBallon.gameObject.activeSelf) CreatureBallon.gameObject.SetActive(true);

    switch (mood)
    {
      case FMood.Unbearable:
      case FMood.Death:
      case FMood.Exhausted:
      case FMood.Soneedbathroom:
      case FMood.Starving:
        CreatureBallon.color = COLOR.SMOKERED;
        break;
      default: CreatureBallon.color = Color.white;
        break;
    }

    switch(job)
    {
      case FPersonalJob.Sleepy:
        InfoImage.sprite = Managers.Resource.Load<Sprite>("creatureSleepy");
        break;
      case FPersonalJob.Excretion:
        InfoImage.sprite = Managers.Resource.Load<Sprite>("creatureExcretion");
        break;
      case FPersonalJob.Hungry:
        InfoImage.sprite = Managers.Resource.Load<Sprite>("creatureHungry");
        break;
      default: break;
    }


  }

  public void InfoReset(Enum job)
  {
    if (!CreatureBallon.gameObject.activeSelf) return;

    CreatureBallon.gameObject.SetActive(false);
  }
}
