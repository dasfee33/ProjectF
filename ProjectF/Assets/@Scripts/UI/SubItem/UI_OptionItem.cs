using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Define;

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

  public void SetInfo(Data.ConsumableItemData data)
  {
    this.data = data;
    GetImage((int)Images.Face).sprite = Managers.Resource.Load<Sprite>(data.Sprite);
    GetText((int)Texts.Name).text = data.Name;
  }

  public void ClickSomething(PointerEventData evt)
  {
    parent.ClickSomething.Invoke(this.data);
  }
}
