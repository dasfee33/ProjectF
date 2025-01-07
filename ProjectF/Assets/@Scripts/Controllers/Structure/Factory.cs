using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class Factory : Structure
{
  public Dictionary<int, float> makeNeedList = new Dictionary<int, float>();
  public Dictionary<int, float> curMakeNeedList;

  public Dictionary<int, float> makeList = new Dictionary<int, float>();
  public Dictionary<int, float> curMakeList;

  private Vector3 dropPos = default;

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

    StructureType = FStructureType.Machine;
    StructureSubType = FStructureSubType.Factory;

    StartCoroutine(CoUpdateAI());
    return true;
  }

  public override void SetInfo(int dataID)
  {
    base.SetInfo(dataID);

    if (data.supplyItemid is not null)
    {
      for (int i = 0; i < data.supplyItemid.Count; i++)
        makeNeedList.Add(data.supplyItemid[i], data.supplyItemMass[i]);

      curMakeNeedList = new Dictionary<int, float>(makeNeedList);
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
    if(CheckMakeIsReady())
      StructureState = FStructureState.Work;
    else
    {
      Worker.ResetJob();

      StructureState = FStructureState.Idle;
      onWorkSomeOne = false;
    }
  }

  private bool CheckMakeIsReady()
  {
    if(curMakeNeedList is not null)
    {
      var saveList = new Dictionary<int, float>();

      foreach (var item in curMakeNeedList)
      {
        if (Worker.SearchHaveList(item.Key))
        {
          float result = Worker.SupplyFromHaveList(item.Key, item.Value);
          saveList.Add(item.Key, result);
          //attackOwner.ResetJob();
        }
      }

      if (saveList.Count > 0)
      {
        foreach (var save in saveList)
          curMakeNeedList[save.Key] -= save.Value;
      }
      bool check = curMakeNeedList.Values.All(value => value <= 0);
      return check;
    }

    return true;
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
