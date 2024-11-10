using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_Research : UI_Popup
{
  public List<UI_ResearchItem> ResearchItems = new List<UI_ResearchItem>();


  public override bool Init()
  {
    if (base.Init() == false) return false;

    SetItem();

    return true;
  }

  private void SetItem()
  {
    for(int i= 0; i < ResearchItems.Count; i++)
    {
      var item = ResearchItems[i];
      item.SetInfo(Managers.Data.ResearchDic[i + RESEARCH_START]);
    } 
   
  }
}
