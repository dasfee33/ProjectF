using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using static Define;

public class Storage : Structure
{
  [SerializedDictionary("item dataID", "item mass")]
  public SerializedDictionary<int, float> storageItem;

  public override FStructureState StructureState
  {
    get { return base.StructureState; }
    set
    {
      if (structureState != value)
      {
        base.StructureState = value;
        switch (value)
        {
          case FStructureState.Idle:
            UpdateAITick = 0f;
            break;
          case FStructureState.WorkStart:
            UpdateAITick = 0f;
            break;
          case FStructureState.Work:
            UpdateAITick = data.WorkTime;
            break;
          case FStructureState.WorkEnd:
            UpdateAITick = 2f;
            break;
        }
      }
    }
  }

  public void AddCapacity(float mass)
  {
    CurCapacity += mass;
  }


  public override bool Init()
  {
    if (base.Init() == false) return false;

    StructureType = FStructureType.Furniture;
    StructureSubType = FStructureSubType.Storage;


    StartCoroutine(CoUpdateAI());
    return true;
  }

  protected override void UpdateIdle()
  {
    onWorkSomeOne = false;
  }

  protected override void UpdateWorkStart()
  {
    onWorkSomeOne = true;
    StructureState = FStructureState.Work;
  }

  protected override void UpdateOnWork()
  {
    if(onWorkSomeOne)
    {
      StructureState = FStructureState.WorkEnd;
    }

    
  }

  protected override void UpdateWorkEnd()
  {
    var haveList = new List<int>();
    foreach(var item in Worker.ItemHaveList)
    {
      if (storageItem.TryGetValue(item.Key, out float value))
      {
        storageItem[item.Key] += item.Value;
        AddCapacity(item.Value);
        Worker.CurrentSupply -= item.Value;
        haveList.Add(item.Key);
      }
      else
      {
        storageItem.Add(item.Key, item.Value);
        AddCapacity(item.Value);
      }
    }

    foreach(var list in haveList)
    {
      Worker.ItemHaveList.Remove(list);
    }
   
    {
      //Worker.CurrentSupply = 0;
      Worker.jobSystem.supplyTargets.Clear();
      Worker.jobSystem.target = null;
      Worker.SetOrAddJobPriority(workableJob, 0, true);
      Worker.Target = null;
      Worker = null;

      StructureState = FStructureState.Idle;
      onWorkSomeOne = false;
    }

  }
}
