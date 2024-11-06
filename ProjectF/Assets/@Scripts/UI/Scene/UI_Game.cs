using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static Define;

public class UI_Game : UI_Scene
{
  enum Buttons
  {
    Image,
    Image2
  }
  enum Images
  {
    PeriodMoon,
  }
  enum Texts
  {
    PeriodText,
  }

  private Image PeriodMoon;
  private TextMeshProUGUI PeriodText;
  private float rotationSpeed; 

  public override bool Init()
  {
    if (base.Init() == false) return false;

    rotationSpeed = 360 / Managers.GameDay.day;

    BindButtons(typeof(Buttons));
    BindImages(typeof(Images));
    BindTexts(typeof(Texts));

    GetButton((int)Buttons.Image).gameObject.BindEvent(Test, FUIEvent.Click);
    GetButton((int)Buttons.Image2).gameObject.BindEvent(Test2, FUIEvent.Click);

    PeriodMoon = GetImage((int)Images.PeriodMoon);
    PeriodText = GetText((int)Texts.PeriodText);

    return true;
  }

  public void Test(PointerEventData evt)
  {
    Time.timeScale *= 2;
  }

  public void Test2(PointerEventData evt)
  {
    Time.timeScale /= 2;
  }

  private void Update()
  {
    PeriodText.text = $"Period\n{Managers.GameDay.Period}";
    PeriodMoon.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
  }
}
