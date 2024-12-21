using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BuildSlider : UI_Base
{
  public Coroutine coBuild;

  public enum Sliders
  {
    Slider,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindSliders(typeof(Sliders));

    return true;
  }

  public void SetInfo(float buildTime)
  {
    coBuild = StartCoroutine(Build(buildTime));
  }

  private IEnumerator Build(float buildTime)
  {
    var timer = 0f;
    var slider = GetSlider((int)Sliders.Slider);
    slider.maxValue = buildTime;

    while(timer < buildTime)
    {
      timer += Time.deltaTime;
      GetSlider((int)Sliders.Slider).value += Time.deltaTime;

      yield return null;
    }
  }
}
