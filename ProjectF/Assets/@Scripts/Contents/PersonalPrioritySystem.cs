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
  private Dictionary<FPersonalJob, float> personalDict = new Dictionary<FPersonalJob, float>();

  public BaseObject target;
  public List<BaseObject> targets = new List<BaseObject>();  

  public KeyValuePair<FPersonalJob, float> CurrentPersonalJob
  {
    get
    {
      foreach (var job in personalDict)
      {
        if (job.Value < 80f) continue;

        targets = Owner.FindsClosestInRange(job.Key, 10f, Managers.Object.Workables, func: Owner.IsValid);

        foreach(var t in targets)
        {
          if (targets != null)
          {
            //작업자가 이미 있는데 그게 내가 아니라면 
            if (t.Worker != null && t.Worker != Owner) continue;
            target = t;
            target.Worker = Owner;
            return job;
          }
        }
      }

      return new KeyValuePair<FPersonalJob, float>(FPersonalJob.None, 0);
    }

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

  private void RefreshJobList(KeyValuePair<FPersonalJob, float> job)
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

  private Dictionary<FPersonalJob, float> DescendingDIct(Dictionary<FPersonalJob, float> dict)
  {
    return dict.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
  }

  private Dictionary<FPersonalJob, float> GetSelectJobList(int count)
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
      tmp = Mathf.Clamp(personalDict[FPersonalJob.Excretion] + 1, 0, 100);

      personalDict[FPersonalJob.Excretion] = tmp;
    }

    //배고픔
    if (personalDict.ContainsKey(FPersonalJob.Hungry))
    {
      var tmp = 0f;
      Mathf.Clamp(Owner.Calories -= 50, 0, Owner.Calories);

      if(Owner.Calories < 500f) tmp = Mathf.Clamp(personalDict[FPersonalJob.Hungry] + 5, 0, 100);
      else if(Owner.Calories < 5000f) tmp = Mathf.Clamp(personalDict[FPersonalJob.Hungry] + 1, 0, 100);

      personalDict[FPersonalJob.Hungry] = tmp;
    }

    //수면 
    if (personalDict.ContainsKey(FPersonalJob.Sleepy))
    {
      var tmp = 0f;
      switch (gameDay.currentTime)
      {
        case FCurrentTime.BeforeSunset: tmp = Mathf.Clamp(personalDict[FPersonalJob.Sleepy] += 5, 0, 100); break;
        case FCurrentTime.Night: tmp = Mathf.Clamp(personalDict[FPersonalJob.Sleepy] += 10, 0, 100); break;
      }

      personalDict[FPersonalJob.Sleepy] = tmp;
    }
  }

}
