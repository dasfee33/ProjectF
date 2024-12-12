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

  public void AddCapacity(int key, float mass)
  {
    if(!storageItem.ContainsKey(key)) storageItem.Add(key, mass);
    else storageItem[key] += mass;

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
    var haveList = new Dictionary<int, float>(); // 임시 저장소. 쓰면 안됌 
    foreach(var item in Worker.ItemHaveList)
    {
      var value = Mathf.Min(item.Value, MaxCapacity - CurCapacity);
      AddCapacity(item.Key, value);
      Managers.Object.AddItem(item.Key, value, this);
      haveList.Add(item.Key, value);
    }

    foreach(var list in haveList)
    {
      Worker.SupplyFromHaveList(list.Key, list.Value);
    }
   
    {
      //Worker.CurrentSupply = 0;
      Worker.ResetJob();
      Worker = null;

      StructureState = FStructureState.Idle;
      onWorkSomeOne = false;
    }

  }
}
