using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_PriorityPopupSelectJob : UI_Base
{
  public Creature _owner;
  private FJob _thisJob;

  public enum DropDowns
  {
    Dropdown,
  }

  public enum Images
  {
    JobImage,
  }

  public enum Texts
  {
    JobName,
  }

  private bool init = true;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindDropDowns(typeof(DropDowns));
    BindImages(typeof(Images));
    BindTexts(typeof(Texts));

    GetDropDown((int)DropDowns.Dropdown).onValueChanged.AddListener(DropDownChanges);
    GetDropDown((int)DropDowns.Dropdown).value = 2;

    return true;
  }

  public void SetInfo(FJob job)
  {
    _thisJob = job;
    GetText((int)Texts.JobName).text = Managers.Game.GetText(job.ToString());
  }

  public void ImageChanges(Sprite sprite)
  {
    GetImage((int)Images.JobImage).sprite = sprite;
  }

  public void DropDownChanges(int index)
  {
    ImageChanges(GetDropDown((int)DropDowns.Dropdown).options[index].image);
    if (!init) Managers.Event.jobPriorityChanged.Invoke(_thisJob, 4 - index);
    else init = !init;
  }
}
