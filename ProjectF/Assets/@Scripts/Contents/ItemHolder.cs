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

  public int stack;


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
    ownerPos = Owner.transform.position;
    dataTemplateID = itemDataId;
    data = Managers.Data.ItemDic[itemDataId];
    currentSprite.sprite = Managers.Resource.Load<Sprite>($"{data.Name}");
    currentSprite.sortingOrder = 19;
    parabolaMotion.SetInfo(0, transform.position, pos, endCallback: Arrived);
    
  }

  private void Arrived()
  {
    //이미 어떤 아이템이 드랍될 경우 그 위치로 떨어지게 하고 스택 증가 
    if (Owner.droppedItem != null)
    {
      if (Owner.droppedItem.stack < Owner.droppedItem.data.maxStack)
      {
        Owner.droppedItem.stack++;
        Managers.Object.Despawn(this);
      }
    }
    else Owner.droppedItem = this;

    //currentSprite.DOFade(0, 3f).OnComplete(() =>
    //{
    //  //if (_data != null)
    //  //{
    //  //  //Acquire item
    //  //}

    //  Managers.Object.Despawn(this);
    //});
  }
}
