using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static Util;

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
    GetText((int)Texts.Name).text = Managers.Game.GetText(data.Name);

    BuildCheck();
  }

  public bool BuildCheck()
  {
    var Image = this.GetComponent<Image>();
    if (!IsBuildAvailable(data.buildItemId, data.buildItemMass, Managers.Object.ItemStorage))
    {
      Image.color = COLOR.SMOKERED;
      return false;
    }
    else
    {
      Image.color = COLOR.WHITE;
      return true;
    }
  }

  private void ClickedItem(PointerEventData evt)
  {
    var toolBase = Managers.Map.Map.GetComponent<ToolBase>();
    if (toolBase == null) return;
    if (toolBase.onBuild)
    {
      Managers.Event.Notice(Managers.Game.GetText("BUILD_NOTDECIDE"));
      return;
    }

    if (!BuildCheck())
    {
      Managers.Event.Notice(Managers.Game.GetText("BUILD_NOTENOUGH"));
      return;
    }

    toolBase.SpawnBuildObject(data);

  }

  
}
