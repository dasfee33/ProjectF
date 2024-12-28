using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_BaseUnlockPopupItemButton : UI_Base
{
  private Image itemImage;
  private int dataID;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    itemImage = this.GetComponent<Image>();
    itemImage.gameObject.BindEvent(ClickItem, FUIEvent.Click);

    return true;
  }

  public void SetInfo(string sprite, int dataID)
  {
    itemImage.sprite = Managers.Resource.Load<Sprite>(sprite);
    this.dataID = dataID;
  }

  public void ClickItem(PointerEventData evt)
  {
    Managers.Event.ShowDescription(rect.localPosition, transform.root, FObjectType.Structure, dataID);
  }
}
