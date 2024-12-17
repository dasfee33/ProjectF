using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[Serializable]
public class StorageItem
{
  [SerializeField]
  private int _id;
  [SerializeField]
  private float _mass;
  [SerializeField]
  private string _label;

  public int id { get { return _id; } set { _id = value; } }
  public float mass { get { return _mass; } set { _mass = value; } }
  public string label { get { return _label; } set { _label = value; } }

  public StorageItem(int id, float mass, string label)
  {
    this.id = id;
    this.mass = mass;
    this.label = label;
  }
}

public class Storage : Structure
{
  //[SerializedDictionary("item dataID", "item mass")]
  //public SerializedDictionary<int, float> storageItem;
  public List<StorageItem> storageItem = new List<StorageItem>();

  public KeyValuePair<int, float> takeItem = new KeyValuePair<int, float>(-1, 0);

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

  public void AddCapacity(int key, float mass, string label = null)
  {
    var item = storageItem.ReturnProperty("id", key) as StorageItem;  
    if(item != null)
    {
      item.mass += mass;
    }
    else storageItem.Add(new StorageItem(key, mass, label));

    CurCapacity += mass;

    for(var i = storageItem.Count - 1; i >= 0; i--)
    {
      if (storageItem[i].mass <= 0) storageItem.RemoveAt(i);
    }
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
    // 빼는 것
    if (takeItem.Key != -1)
    {
      var value = 0f;
      var item = storageItem.ReturnProperty("id", takeItem.Key) as StorageItem;

      if(item != null)
      {
        value = Mathf.Min(takeItem.Value, item.mass);
      }
      var realValue = Worker.AddHaveList(takeItem.Key, value);
      AddCapacity(takeItem.Key, -realValue);

    }
    else
    {
      // 넣는 것
      var haveList = new List<StorageItem>(); // 임시 저장소. 쓰면 안됌 
      foreach (var item in Worker.ItemHaveList)
      {
        var value = Mathf.Min(item.mass, MaxCapacity - CurCapacity);
        AddCapacity(item.id, value, item.label);
        haveList.Add(new StorageItem(item.id, value, item.label));
      }

      foreach (var list in haveList)
      {
        Worker.SupplyFromHaveList(list.id, list.mass, list.label);
      }
    }

    //Worker.CurrentSupply = 0;
    Worker.ResetJob();
    Worker = null;

    takeItem = new KeyValuePair<int, float>(-1, 0);
    StructureState = FStructureState.Idle;
    onWorkSomeOne = false;
  }
}
