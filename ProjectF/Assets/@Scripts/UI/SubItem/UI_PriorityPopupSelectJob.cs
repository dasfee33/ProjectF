using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PriorityPopupSelectJob : UI_Base
{

  public enum Texts
  {
    JobName,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindTexts(typeof(Texts));

    return true;
  }

  public void SetInfo(string jobName)
  {
    GetText((int)Texts.JobName).text = jobName;
  }
}
