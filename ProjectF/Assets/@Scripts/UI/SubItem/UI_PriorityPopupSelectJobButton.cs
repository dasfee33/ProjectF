using System;
using UnityEngine.EventSystems;
using static Define;
using UnityEngine;
using System.Collections.Generic;

public class UI_PriorityPopupSelectJobButton : UI_Base
{
  public Creature _owner;

  public enum Buttons
  {
    SelectButton,
  }

  public enum Images
  {
    ButtonImage,
  }

  public FJobSelectEvent SelectEvent
  {
    get { return selectEvent; }
    set
    {
      selectEvent = value;
      jobPriority = (int)value * 10;
      GetImage((int)Images.ButtonImage).sprite = SpritePool[(int)selectEvent];
    }
  }

  public List<Sprite> SpritePool;

  private int poolPoint = 2;
  public float jobPriority = 0f;
  public FJob job;

  public FJobSelectEvent selectEvent = FJobSelectEvent.None;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindButtons(typeof(Buttons));
    BindImages(typeof(Images));

    GetButton((int)Buttons.SelectButton).gameObject.BindEvent(ClickButton, FUIEvent.Click);

    return true;
  }

  public void ClickButton(PointerEventData evt)
  {
    SetPriority(1);
  }

  public void SetPriority(int count, bool init = false)
  {
    _ = init == false ? poolPoint += count : poolPoint = count;

    SelectEvent = (FJobSelectEvent)(poolPoint % SpritePool.Count);
    _owner.SetOrAddJobPriority(job, jobPriority, true);
  }

  public void SetInfo(FJob _job)
  {
    job = _job;
    jobPriority = _owner.JobDic[_job].Priority;
    SetPriority((int)jobPriority / 10, true);
  }
}
