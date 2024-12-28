using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Define;

public class UI_DescriptionPopup : UI_Popup
{
  public enum Texts
  {
    DescriptionText,
    MaterialText,
    BuildTimeText,
    QualityText,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindTexts(typeof(Texts));

    Managers.Event.description -= SetInfo;
    Managers.Event.description += SetInfo;

    return true;
  }

  public void SetInfo(FObjectType type, int dataID)
  {
    switch(type)
    {
      case FObjectType.Structure:
        var data = Managers.Data.StructDic[dataID];
        GetText((int)Texts.DescriptionText).text = data.DescriptionTextID;
        GetText((int)Texts.MaterialText).text = "재료 : ";
        for (int i = 0; i < data.buildItemId.Count; i++)
        {
          var buildItem = data.buildItemId[i];
          var buildItemMass = data.buildItemMass[i];
          var item = Managers.Data.ItemDic[buildItem];

          GetText((int)Texts.MaterialText).text += $"{item.Name} : {buildItemMass}Kg/ ";
        }
        GetText((int)Texts.BuildTimeText).text = $"건설시간 : {data.BuildTime}sec";
        GetText((int)Texts.QualityText).text = "품질 : 매우 나쁨!"; //FIXME

        break;
      default: break;
    }
  }
}
