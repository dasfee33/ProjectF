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
  public string label;
  public bool isDropped = false;

  public float takeItemMass = 0f;

  public override bool Init()
  {
    if (base.Init() == false)
      return false;
    guid = Guid.NewGuid();

    ObjectType = FObjectType.ItemHolder;
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
    maxStack = data.maxStack;
    defaultMass = data.Mass;
    label = data.Label;
    if(dropPos != default(Vector3))  parabolaMotion.SetInfo(0, transform.position, dropPos, endCallback: Arrived);

    UpdateJob();
  }

  private void UpdateJob()
  {
    switch(label)
    {
      case "Material":
      case "Seed":
        workableJob = FJob.Supply; break;
      case "Food":
        workableJob = FPersonalJob.Hungry; break;
    }
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
        Managers.Object.AddItem(Owner.droppedItem.dataTemplateID, defaultMass);
        Owner.droppedItem.RefreshStack(1);
        Managers.Object.Despawn(this);
      }
    }
    else
    {
      isDropped = true;
      Owner.droppedItem = this;
      Managers.Object.AddItem(Owner.droppedItem.dataTemplateID, defaultMass);
      stack += 1;
      mass = (defaultMass * stack);
    }
  }

  public void ReduceCapacity(float takeMass)
  {
    mass -= takeMass;
    if (mass > 0)
    {
      stack = (int)(mass / defaultMass);
      takeItemMass = 0f;
    }
    else Managers.Object.Despawn(this);
  }

  public override void OnDamaged(BaseObject attacker)
  {
    var attackOwner = attacker as Creature;

    Managers.Event.CreatureAction(attackOwner, Managers.Game.GetText("EVENT_CREATUREACTION", data.DescirptionTextID));

    var takeMass = Mathf.Min(takeItemMass, mass);
    if (takeMass == 0) takeMass = mass;

    takeMass = attackOwner.AddHaveList(dataTemplateID, takeMass, label);
    attackOwner.ResetJob();

    ReduceCapacity(takeMass);
  }
}
