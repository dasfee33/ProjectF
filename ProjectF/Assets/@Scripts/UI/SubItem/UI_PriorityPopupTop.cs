using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_PriorityPopupTop : UI_Base
{
  public Data.CreatureData data;

  public enum Texts
  {
    Name,
  }

  public bool fix = false;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindTexts(typeof(Texts));
    
;

    return true;
  }

  //사진 / 이름 설정
  public void SetInfo()
  {
    GetText((int)Texts.Name).text = data.Name;
  }
}
