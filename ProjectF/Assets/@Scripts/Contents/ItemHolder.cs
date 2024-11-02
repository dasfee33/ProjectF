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
    //떨어진 곳에 같은 아이템이 있는 경우 스택을 증가시킴 / 아니면 생성
    var posObj = Managers.Map.GetObject(dropPos);
    if (posObj != null && posObj.dataTemplateID == this.dataTemplateID)
    {
      var posObjItemHolder = posObj.GetComponent<ItemHolder>();
      if (posObjItemHolder.guid == guid) return;

      if(posObjItemHolder.stack < posObjItemHolder.data.maxStack)
      {
        posObjItemHolder.stack++;
        Managers.Object.Despawn(this, ownerPos);
      }
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
}
