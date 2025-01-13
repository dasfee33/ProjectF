using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Define;
using System;

public class UI_OptionItem : UI_Base
{
  public enum Images
  {
    Face,
  }

  public enum Texts
  {
    Name,
  }

  public UI_Option parent;
  private Data.ConsumableItemData data;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindImages(typeof(Images));
    BindTexts(typeof(Texts));

    this.GetComponent<Button>().gameObject.BindEvent(ClickSomething, FUIEvent.Click);

    return true;
  }

  public void SetInfo(Data.ConsumableItemData data, UI_Option parent)
  {
    this.parent = parent;
    this.data = data;
    GetImage((int)Images.Face).sprite = Managers.Resource.Load<Sprite>(data.Sprite);
    GetText((int)Texts.Name).text = Managers.Game.GetText(data.Name);

    RefreshFace();
  }

  private void RefreshFace()
  {
    var structure = parent.Owner as Structure;

    for(int i = 0; i < structure.data.makeItemid.Count; i++)
    {
      var makeItem = structure.data.makeItemid[i];

      if (Managers.Object.ItemStorage.TryGetValue(makeItem, out var value))
      {
        if (value.mass < structure.data.makeItemMass[i])
        {
          GetImage((int)Images.Face).color = COLOR.SMOKERED;
          return;
        }
      }
    }
    GetImage((int)Images.Face).color = COLOR.WHITE;
  }

  public void ClickSomething(PointerEventData evt)
  {
    parent.ClickSomething.Invoke(this.data);
  }
}
