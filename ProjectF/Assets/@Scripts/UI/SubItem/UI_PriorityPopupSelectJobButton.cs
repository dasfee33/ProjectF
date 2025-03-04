using System;
using UnityEngine.EventSystems;
using static Define;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
      buttonImage.sprite = SpritePool[(int)selectEvent];
    }
  }

  public List<Sprite> SpritePool;

  private int poolPoint = 2;
  public float jobPriority = 0f;
  public FJob job;

  public FJobSelectEvent selectEvent = FJobSelectEvent.None;
  private Image buttonImage;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindButtons(typeof(Buttons));
    BindImages(typeof(Images));

    GetButton((int)Buttons.SelectButton).gameObject.BindEvent(ClickButton, FUIEvent.Click);
    buttonImage = GetImage((int)Images.ButtonImage);

    Managers.Event.jobPriorityChanged -= JobPriorityChanged;
    Managers.Event.jobPriorityChanged += JobPriorityChanged;

    return true;
  }

  public void JobPriorityChanged(FJob job, int priority)
  {
    if (this.job != job) return;

    SetPriority(priority, true);
  }

  public void ClickButton(PointerEventData evt)
  {
    SetPriority(1);
  }

  public void OnDestroy()
  {
    Managers.Event.jobPriorityChanged -= JobPriorityChanged; //fake null
  }

  public void SetPriority(int count, bool set = false)
  {
    _ = set == false ? poolPoint += count : poolPoint = count;

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
