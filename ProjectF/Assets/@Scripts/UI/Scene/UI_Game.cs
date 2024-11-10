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
    Fast,
    Slow,

    Base,
    Furniture,
    Pipe,
    Electronic,
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
  private UI_BuildPopup build;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    rotationSpeed = 360 / Managers.GameDay.day;

    BindButtons(typeof(Buttons));
    BindImages(typeof(Images));
    BindTexts(typeof(Texts));

    GetButton((int)Buttons.Fast).gameObject.BindEvent(Test, FUIEvent.Click);
    GetButton((int)Buttons.Slow).gameObject.BindEvent(Test2, FUIEvent.Click);

    GetButton((int)Buttons.Base).gameObject.BindEvent(ClickBase, FUIEvent.Click);
    GetButton((int)Buttons.Furniture).gameObject.BindEvent(ClickFurniture, FUIEvent.Click);
    GetButton((int)Buttons.Pipe).gameObject.BindEvent(ClickPipe, FUIEvent.Click);
    GetButton((int)Buttons.Electronic).gameObject.BindEvent(ClickElectronic, FUIEvent.Click);

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

  private void ClickBase(PointerEventData evt)
  {
    if(build == null) build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
    build.Refresh();
  }

  private void ClickFurniture(PointerEventData evt)
  {
    if (build == null) build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
    build.Refresh();

  }

  private void ClickPipe(PointerEventData evt)
  {
    if (build == null) build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
    build.Refresh();

  }

  private void ClickElectronic(PointerEventData evt)
  {
    if (build == null) build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
    build.Refresh();
  }

  private void Update()
  {
    PeriodText.text = $"Period\n{Managers.GameDay.Period}";
    PeriodMoon.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
  }
}
