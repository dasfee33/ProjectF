using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class JobSystem : InitBase
{
  public Creature Owner { get; protected set; }
  public FJob Job { get; protected set; }

  private int jobCount = 10;
  private Dictionary<FJob, jobDicValue> jobDict = new Dictionary<FJob, jobDicValue>();

  public BaseObject target;
  public BaseObject supplyTarget;
  public List<BaseObject> targets = new List<BaseObject>();
  public List<BaseObject> supplyTargets = new List<BaseObject>();

  public KeyValuePair<FJob, jobDicValue> CurrentJob
  {
    get
    {
      foreach(var job in jobDict)
      {
        if (!job.Value.IsAble) continue;
        targets = Owner.FindsClosestInRange(job.Key, 10f, Managers.Object.Workables, func: Owner.IsValid);

        //if (targets.Count <= 0 || targets == null) Owner.SetJobIsAble(Job, false);
        //else Owner.SetJobIsAble(Job, true);

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
      return new KeyValuePair<FJob, jobDicValue>(FJob.None, new jobDicValue(0, false));
    }
    
  }

  public BaseObject CurrentRootJob
  {
    get
    {
      if(supplyTargets.All(item => item == null))
      {
        if(supplyTargets.Count <= 0)
          supplyTargets = Owner.FindsClosestInRange(FJob.Supply, 10f, Managers.Object.ItemHolders, func: Owner.IsValid);
      }
        

      foreach (var t in supplyTargets)
      {
        if (t == null) continue;
        var itemHolder = t.GetComponent<ItemHolder>();
        if (Owner.CurrentSupply + itemHolder.mass > Owner.SupplyCapacity) continue;
        if (!itemHolder.isDropped) continue;
        if (itemHolder.stack <= 0) continue;

        if (supplyTargets != null)
        {
          if (t.Worker != Owner.Target.Worker) t.Worker = null;
          //작업자가 이미 있는데 그게 내가 아니라면 포기? 
          if (t.Worker != null && t.Worker != Owner) continue;
          supplyTarget = t;
          t.Worker = Owner;
          return t;
        }
      }
      return null;
    }
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;
    Owner = this.GetComponent<Creature>();

    Owner.jobChanged -= RefreshJobList;
    Owner.jobChanged += RefreshJobList;
    Owner.JobDic = DescendingDIct(Owner.JobDic);
    MakeJobList();

    return true;
  }

  private void MakeJobList()
  {
    jobDict = GetSelectJobList(jobCount);
  }

  private void RefreshJobList(KeyValuePair<FJob, jobDicValue> job)
  {
    if (jobDict.ContainsKey(job.Key))
    {
      jobDict[job.Key] = job.Value;
      jobDict = DescendingDIct(jobDict);
    }
    else
    {
      jobDict.Add(job.Key, job.Value);
      jobDict = DescendingDIct(jobDict);
      jobDict.Remove(jobDict.Last().Key);
    }
  }

  private Dictionary<FJob, jobDicValue> DescendingDIct(Dictionary<FJob, jobDicValue> dict)
  {
    return dict.OrderByDescending(pair => pair.Value.Priority).ToDictionary(pair => pair.Key, pair => pair.Value);
  }

  private Dictionary<FJob, jobDicValue> GetSelectJobList(int count)
  {
    var sortDict = Owner.JobDic.Take(jobCount).ToDictionary(pair => pair.Key, pair => pair.Value); 
    return sortDict;
  }

}
