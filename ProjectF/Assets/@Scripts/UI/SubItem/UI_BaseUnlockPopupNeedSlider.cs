using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BaseUnlockPopupNeedSlider : UI_Base
{
  public enum Texts
  {
    ResearchGauge,
  }

  private int level;
  private int curValue;
  private int maxValue;

  public Slider slider;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindTexts(typeof(Texts));

    slider = this.GetComponent<Slider>();

    return true;
  }

  public void SetInfo(int level, int curValue, int maxValue)
  {
    this.level = level;
    this.curValue = curValue;
    this.maxValue = maxValue;

    slider.maxValue = maxValue;
    slider.value = Mathf.Clamp(curValue, 0, maxValue);

    GetText((int)Texts.ResearchGauge).text = $"{slider.value}/{slider.maxValue}";
  }
}
