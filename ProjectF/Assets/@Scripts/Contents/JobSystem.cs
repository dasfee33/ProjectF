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
  private Dictionary<FJob, float> jobDict = new Dictionary<FJob, float>();

  public BaseObject target;

  public KeyValuePair<FJob, float> CurrentJob
  {
    get
    {
      foreach(var job in jobDict)
      {
        target = Owner.FindClosestInRange(job.Key, 10f, Managers.Object.Workables, func: Owner.IsValid);
        
        if (target != null)
        {
          //작업자가 이미 있는데 그게 내가 아니라면 
          if (target.Worker != null && target.Worker != Owner) continue;
          target.Worker = Owner;
          return job;
        }
      }

      return new KeyValuePair<FJob, float>(FJob.None, 0);
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

  private void RefreshJobList(KeyValuePair<FJob, float> job)
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

  private Dictionary<FJob, float> DescendingDIct(Dictionary<FJob, float> dict)
  {
    return dict.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
  }

  public Dictionary<FJob, float> GetSelectJobList(int count)
  {
    var sortDict = Owner.JobDic.Take(10).ToDictionary(pair => pair.Key, pair => pair.Value); 
    return sortDict;
  }

}
