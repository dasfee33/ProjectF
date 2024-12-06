using System;
using UnityEngine;
using DG.Tweening;
using static Define;
using Data;

public class ItemHolder : BaseObject
{
  public BaseObject Owner { get; set; }
  public ItemData data;
  private SpriteRenderer currentSprite;
  private ParabolaMotion parabolaMotion;
  private Vector3 dropPos;
  private Vector3 ownerPos;
  public Guid guid { get; set; }

  public int maxStack;
  public int stack;
  public float defaultMass;
  public float mass;
  public bool isDropped = false;

  public override bool Init()
  {
    if (base.Init() == false)
      return false;
    guid = Guid.NewGuid();

    ObjectType = FObjectType.ItemHolder;
    //FIXME
    workableJob = FJob.Supply;
    currentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
    parabolaMotion = gameObject.GetOrAddComponent<ParabolaMotion>();

    return true;
  }

  public void SetInfo(int itemHolderId, int itemDataId, Vector3 pos)
  {
    dropPos = pos;
    if(Owner != null) ownerPos = Owner.transform.position;
    dataTemplateID = itemDataId;
    data = Managers.Data.ItemDic[itemDataId];
    currentSprite.sprite = Managers.Resource.Load<Sprite>($"{data.Name}");
    currentSprite.sortingOrder = 19;
    maxStack = data.maxStack;
    defaultMass = data.Mass;
    if(dropPos != default(Vector3))  parabolaMotion.SetInfo(0, transform.position, dropPos, endCallback: Arrived);

  }

  public void RefreshStack(int a)
  {
    stack += a;
    mass = (defaultMass * stack);
  }

  private void Arrived()
  {
    //이미 어떤 아이템이 드랍될 경우 그 위치로 떨어지게 하고 스택 증가 
    if (Owner.droppedItem != null && Owner.droppedItem.dataTemplateID == this.dataTemplateID)
    {
      if (Owner.droppedItem.stack < Owner.droppedItem.data.maxStack)
      {
        Owner.droppedItem.RefreshStack(1);
        Managers.Object.AddItem(Owner.droppedItem.dataTemplateID, defaultMass);
        Managers.Object.Despawn(this);
      }
    }
    else
    {
      isDropped = true;
      Owner.droppedItem = this;
      stack += 1;
      mass = (defaultMass * stack);
    }

    //currentSprite.DOFade(0, 3f).OnComplete(() =>
    //{
    //  //if (_data != null)
    //  //{
    //  //  //Acquire item
    //  //}

    //  Managers.Object.Despawn(this);
    //});
  }

  public override void OnDamaged(BaseObject attacker)
  {
    var attackOwner = attacker as Creature;
    if(attackOwner.SupplyCapacity >= attackOwner.CurrentSupply + mass)
    {
      attackOwner.AddHaveList(dataTemplateID, mass);
      //인벤토리 할때 같이 
      //if (attackOwner.SupplyStorage.Count > 0)
      //{
      //  foreach (var item in attackOwner.SupplyStorage)
      //  {
      //    if (item.dataTemplateID == this.dataTemplateID)
      //    {
      //      var itemHolder = item.GetComponent<ItemHolder>();
      //      int availSpace = itemHolder.maxStack - itemHolder.stack;

      //      if (this.stack <= availSpace)
      //      {
      //        itemHolder.stack += this.stack;
      //      }
      //      else
      //      {
      //        itemHolder.stack = maxStack;
      //        itemHolder.stack = this.stack - availSpace;
      //        attackOwner.SupplyStorage.Add(itemHolder);
      //      }
      //    }
      //  }
      //}
      //else attackOwner.SupplyStorage.Add(this);
      Managers.Object.Despawn(this);
    }
    

  }
}
