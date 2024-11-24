using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindImages(typeof(Images));
    BindTexts(typeof(Texts)); 

    return true;
  }

  public void SetInfo(Data.ItemData data)
  {
    GetImage((int)Images.Face).sprite = Managers.Resource.Load<Sprite>(data.Sprite);
    GetText((int)Texts.Name).text = data.Name;
  }
}
