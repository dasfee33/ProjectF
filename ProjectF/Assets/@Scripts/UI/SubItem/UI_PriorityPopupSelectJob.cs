using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_PriorityPopupSelectJob : UI_Base
{
  public Creature _owner;
  private FJob _thisJob;

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

  public void SetInfo(FJob job)
  {
    _thisJob = job;
    GetText((int)Texts.JobName).text = job.ToString();

  }
}
