using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class PersonalPrioritySystem : InitBase
{
  public Creature Owner { get; protected set; }
  public FPersonalJob Job { get; protected set; }

  private int jobCount = 10;

  private GameDayManager gameDay;
  private Dictionary<FPersonalJob, jobDicValue> personalDict = new Dictionary<FPersonalJob, jobDicValue>();

  public BaseObject target;
  public List<BaseObject> targets = new List<BaseObject>();

  public KeyValuePair<FPersonalJob, jobDicValue> CurrentPersonalJob
  {
    get
    {
      foreach (var job in personalDict)
      {
        if (!job.Value.IsAble) continue;
        if (job.Value.Priority < 60f)
        {
          Managers.Event.MoodStable(Owner, job.Key);
          continue;
        }

        targets = Owner.FindsClosestInRange(job.Key, 10f, Managers.Object.Workables, func: Owner.IsValid);

        foreach (var t in targets)
        {
          if (targets != null)
          {
            //작업자가 이미 있는데 그게 내가 아니라면 
            if (t.Worker != null && t.Worker != Owner) continue;
            target = t;
            target.Worker = Owner;
            UpdateMood(job.Key, job.Value.Priority);
            return job;
          }
        }
      }

      return new KeyValuePair<FPersonalJob, jobDicValue>(FPersonalJob.None, new jobDicValue(0, false));
    }

  }

  private void UpdateMood(FPersonalJob job, float value)
  {
    FMood mood = FMood.None;
    switch(job)
    {
      case FPersonalJob.None:
        mood = FMood.None;
        break;
      case FPersonalJob.Excretion:
        if (value < 80f) mood = FMood.Needbathroom;
        else mood = FMood.Soneedbathroom;
        break;
      case FPersonalJob.Hungry:
        if (value < 80f) mood = FMood.Hungry;
        else mood = FMood.Starving;
        break;
      case FPersonalJob.Sleepy:
        if (value < 80f) mood = FMood.Sleepy;
        else mood = FMood.Exhausted;
        break;
    }
    if (Owner.MoodState == mood) return;

    Managers.Event.MoodChanged(Owner, job, mood);
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    Owner = this.GetComponent<Creature>();
    personalDict = Owner.PersonalDic;

    gameDay = Managers.GameDay;
    gameDay.timeChanged -= RefreshNeeds;
    gameDay.timeChanged += RefreshNeeds;

    Owner.pjobChanged -= RefreshJobList;
    Owner.pjobChanged += RefreshJobList;
    Owner.PersonalDic = DescendingDIct(Owner.PersonalDic);

    MakeJobList();

    return true;
  }

  private void MakeJobList()
  {
    personalDict = GetSelectJobList(jobCount);
  }

  private void RefreshJobList(KeyValuePair<FPersonalJob, jobDicValue> job)
  {
    if (personalDict.ContainsKey(job.Key))
    {
      personalDict[job.Key] = job.Value;
      personalDict = DescendingDIct(personalDict);
    }
    else
    {
      personalDict.Add(job.Key, job.Value);
      personalDict = DescendingDIct(personalDict);
      personalDict.Remove(personalDict.Last().Key);
    }
  }

  private Dictionary<FPersonalJob, jobDicValue> DescendingDIct(Dictionary<FPersonalJob, jobDicValue> dict)
  {
    return dict.OrderByDescending(pair => pair.Value.Priority).ToDictionary(pair => pair.Key, pair => pair.Value);
  }

  private Dictionary<FPersonalJob, jobDicValue> GetSelectJobList(int count)
  {
    var sortDict = personalDict.Take(10).ToDictionary(pair => pair.Key, pair => pair.Value);
    return sortDict;
  }

  public void RefreshNeeds()
  {
    if (Owner == null) return;
    //배설
    if (personalDict.ContainsKey(FPersonalJob.Excretion))
    {
      var tmp = 0f;
      tmp = Mathf.Clamp(personalDict[FPersonalJob.Excretion].Priority + 1, 0, 100);

      personalDict[FPersonalJob.Excretion].Priority = tmp;
    }

    //배고픔
    if (personalDict.ContainsKey(FPersonalJob.Hungry))
    {
      var tmp = 0f;
      Mathf.Clamp(Owner.Calories -= 2, 0, Owner.Calories);

      if (Owner.Calories < 500f) tmp = Mathf.Clamp(personalDict[FPersonalJob.Hungry].Priority + 5, 0, 100);
      else if (Owner.Calories < 5000f) tmp = Mathf.Clamp(personalDict[FPersonalJob.Hungry].Priority + 1, 0, 100);
      else if(Owner.Calories >= 5000f) tmp = Mathf.Clamp(personalDict[FPersonalJob.Hungry].Priority - 1, 0, 100);
      else if(Owner.Calories >= 10000f) tmp = Mathf.Clamp(personalDict[FPersonalJob.Hungry].Priority - 5, 0, 100);

      personalDict[FPersonalJob.Hungry].Priority = tmp;
    }

    //수면 
    if (personalDict.ContainsKey(FPersonalJob.Sleepy))
    {
      var tmp = 0f;
      switch (gameDay.currentTime)
      {
        case FCurrentTime.BeforeSunset: tmp = Mathf.Clamp(personalDict[FPersonalJob.Sleepy].Priority += 5, 0, 100); break;
        case FCurrentTime.Night: tmp = Mathf.Clamp(personalDict[FPersonalJob.Sleepy].Priority += 10, 0, 100); break;
      }

      personalDict[FPersonalJob.Sleepy].Priority = tmp;
    }
  }

}
