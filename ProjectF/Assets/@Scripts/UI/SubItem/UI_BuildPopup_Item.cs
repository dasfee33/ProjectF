using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_BuildPopup_Item : UI_Base
{
  private int objId = 0;
  private Data.StructureData data;

  public enum Buttons
  {
    Face,
  }

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

    BindButtons(typeof(Buttons));
    BindImages(typeof(Images));
    BindTexts(typeof(Texts));

    GetButton((int)Buttons.Face).gameObject.BindEvent(ClickedItem, FUIEvent.Click);

    return true;
  }

  public void SetInfo(int dataID)
  {
    objId = dataID;
    data = Managers.Data.StructDic[dataID];
    GetImage((int)Images.Face).sprite = Managers.Resource.Load<Sprite>(data.Sprite);
    GetText((int)Texts.Name).text = data.Name;
  }

  private void ClickedItem(PointerEventData evt)
  {
    var toolBase = Managers.Map.Map.GetComponent<ToolBase>();
    if (toolBase == null) return;
    toolBase.objData = data;
    toolBase.isBuild = true;

  }

  
}
