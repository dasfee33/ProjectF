using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Kitchen : Structure
{
  public Dictionary<int, float> makeNeedList = new Dictionary<int, float>();
  public Dictionary<int, float> curMakeNeedList;

  public Dictionary<int, float> makeList = new Dictionary<int, float>();
  public Dictionary<int, float> curMakeList;

  public Dictionary<int, float> needList = new Dictionary<int, float>();
  public Dictionary<int, float> curNeedList;

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
            UpdateAITick = 0f;
            break;
        }
      }
    }
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    StructureType = FStructureType.Cook;
    StructureSubType = FStructureSubType.Kitchen;

    StartCoroutine(CoUpdateAI());
    return true;
  }

  public override void SetInfo(int dataID)
  {
    base.SetInfo(dataID);

    if (data.supplyItemid is not null)
    {
      for (int i = 0; i < data.supplyItemid.Count; i++)
        needList.Add(data.supplyItemid[i], data.supplyItemMass[i]);

      curNeedList = new Dictionary<int, float>(needList);
    }

    if (data.makeItemid is not null)
    {
      for (int i = 0; i < data.makeItemid.Count; i++)
        makeList.Add(data.makeItemid[i], data.makeItemMass[i]);

      curMakeList = new Dictionary<int, float>(makeList);
    }


  }

  protected override void UpdateIdle()
  {
    onWorkSomeOne = false;
  }

  protected override void UpdateWorkStart()
  {
    if(workableJob is FJob.Supply)
    {
      var saveList = new Dictionary<int, float>();

      foreach (var item in curNeedList)
      {
        if (Worker.SearchHaveList(item.Key))
        {
          float result = Worker.SupplyFromHaveList(item.Key, item.Value);
          saveList.Add(item.Key, result);
        }
      }

      if (saveList.Count > 0)
      {
        foreach (var save in saveList)
          curNeedList[save.Key] -= save.Value;
      }

      CheckIsOn();
      Worker.ResetJob();
    }
    else if(workableJob is FJob.Cook) 
    {
      StructureState = FStructureState.Work;
    }
  }

  private void CheckIsOn()
  {
    var needComplete = curNeedList.Values.All(value => value <= 0);
    if (needComplete)
    {
      workableJob = FJob.Cook;
      Worker.SetJobIsAble(FJob.Cook, true);
      StructureState = FStructureState.On;
    }
    else return;
  }

  protected override void UpdateOnWork()
  {
    if (!onWorkSomeOne)
    {
      UpdateAITick = data.WorkTime;
      onWorkSomeOne = true;
    }
    else
    {
      StructureState = FStructureState.WorkEnd;
    }


  }

  protected override void UpdateWorkEnd()
  {
    MakeItem();

    Worker.ResetJob();

    StructureState = FStructureState.Idle;
    onWorkSomeOne = false;

    //TODO
  }

  private void MakeItem()
  {
    if (droppedItem is null)
    {
      Vector3 rand = new Vector3(transform.position.x + Random.Range(-2f, -5f) * 0.1f, transform.position.y);
      Vector3 rand2 = new Vector3(transform.position.x + Random.Range(2f, 5f) * 0.1f, transform.position.y);
      dropPos = Random.value < 0.5 ? rand : rand2;
    }

    //TEMP
    var itemKey = 0;
    foreach (var t in makeList.Keys)
    {
      itemKey = t;
      break;
    }

    Managers.Object.Spawn<ItemHolder>(transform.position, itemKey, addToCell: false, dropPos: dropPos, Owner: this);

  }
}
