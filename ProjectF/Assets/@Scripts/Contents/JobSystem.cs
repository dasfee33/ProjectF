using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public struct PQJob : IComparable<PQJob>
{
  public Vector3Int pos;
  public FJob job;
  public int value;

  public int CompareTo(PQJob other)
  {
    if (value == other.value)
      return 0;
    return value < other.value ? 1 : -1;
  }
}

public class JobSystem : InitBase
{
  public Creature Owner { get; protected set; }
  public FJob Job { get; protected set; }

  private KeyValuePair<FJob, int> jobChangePair;

  public event Action<KeyValuePair<FJob, int>> jobChanged;
  public KeyValuePair<FJob, int> JobChanged
  {
    get { return jobChangePair; }
    set
    {
      if(!jobChangePair.Equals(value))
      {
        jobChangePair = value;
        jobChanged?.Invoke(value);
      }
    }
  }

  private Dictionary<FJob, int> OwnerJobDic;
  private PriorityQueue<PQJob> pqj = new PriorityQueue<PQJob>();
  private int jobCount = 10;

  public override bool Init()
  {
    if (base.Init() == false) return false;
    Owner = this.GetComponent<Creature>();
    OwnerJobDic = Owner.JobDic;

    jobChanged -= JobListRefresh;
    jobChanged += JobListRefresh;

    MakeJobList();

    return true;
  }

  private void MakeJobList()
  {
    for (int i = 0; i < jobCount; i++)
    {
      {
        FJob job = Owner.SelectJob();
        //pqj.Push(new PQJob() { key = job, value = OwnerJobDic[job] });
      }
    }
  }

  private void JobListRefresh(KeyValuePair<FJob, int> job)
  {
    pqj.Push(new PQJob());
  }

  private void SearchTarget()
  {

  }
}
