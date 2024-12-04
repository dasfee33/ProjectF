using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static Define;
using System;

public class UI_Game : UI_Scene
{
  enum Objects
  {
    UI_Info,
    UI_Option,
  }

  enum Buttons
  {
    //Dev
    Fast,
    Slow,
    SaveData,
    UpdateData,

    Base,
    Furniture,
    Pipe,
    Electronic,
    Station,
    Cook,

    Priority,
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

    SetSafeArea(FSetUISafeArea.All);

    rotationSpeed = 360 / Managers.GameDay.day;

    BindButtons(typeof(Buttons));
    BindImages(typeof(Images));
    BindTexts(typeof(Texts));
    BindObjects(typeof(Objects));

    GetButton((int)Buttons.Fast).gameObject.BindEvent(Test, FUIEvent.Click);
    GetButton((int)Buttons.Slow).gameObject.BindEvent(Test2, FUIEvent.Click);
    GetButton((int)Buttons.SaveData).gameObject.BindEvent(SaveData, FUIEvent.Click);
    GetButton((int)Buttons.UpdateData).gameObject.BindEvent(UpdateData, FUIEvent.Click);

    GetButton((int)Buttons.Base).gameObject.BindEvent(ClickBase, FUIEvent.Click);
    GetButton((int)Buttons.Furniture).gameObject.BindEvent(ClickFurniture, FUIEvent.Click);
    GetButton((int)Buttons.Pipe).gameObject.BindEvent(ClickPipe, FUIEvent.Click);
    GetButton((int)Buttons.Electronic).gameObject.BindEvent(ClickElectronic, FUIEvent.Click);
    GetButton((int)Buttons.Station).gameObject.BindEvent(ClickStation, FUIEvent.Click);
    GetButton((int)Buttons.Cook).gameObject.BindEvent(ClickCook, FUIEvent.Click);

    GetButton((int)Buttons.Priority).gameObject.BindEvent(ClickPriority, FUIEvent.Click);


    PeriodMoon = GetImage((int)Images.PeriodMoon);
    PeriodText = GetText((int)Texts.PeriodText);

    Managers.FInput.touchObject -= CallbackSomething;
    Managers.FInput.touchObject += CallbackSomething;

    return true;
  }

  private void UpdateData(PointerEventData evt)
  {
    Managers.Game.GameDataUpdate();
  }

  private void SaveData(PointerEventData evt)
  {
    Managers.Game.GameDataInsert();
  }

  public void CallbackSomething(BaseObject obj)
  {
    var info = GetObject((int)Objects.UI_Info).gameObject.GetComponent<UI_Info>();
    var option = GetObject((int)Objects.UI_Option).gameObject.GetComponent<UI_Option>();

    if (obj.Option && !option.gameObject.activeSelf) option.gameObject.SetActive(true);
    if (!info.gameObject.activeSelf) info.gameObject.SetActive(true);

    option.SetInfo(obj);
    info.SetInfo(obj, option);
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
    if (build == null)
    {
      build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
      build.Refresh(evt.pointerClick.name);
    }
    else
    {
      if (build.isActiveAndEnabled && evt.pointerClick.name.Equals(build.current)) build.gameObject.SetActive(false);
      else build.gameObject.SetActive(true);
      build.Refresh(evt.pointerClick.name);
    }
  }

  private void ClickFurniture(PointerEventData evt)
  {
    if(build == null)
    {
      build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
      build.Refresh(evt.pointerClick.name);
    }
    else
    {
      if (build.isActiveAndEnabled && evt.pointerClick.name.Equals(build.current)) build.gameObject.SetActive(false);
      else build.gameObject.SetActive(true);
      build.Refresh(evt.pointerClick.name);
    }

  }

  private void ClickPipe(PointerEventData evt)
  {
    if (build == null)
    {
      build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
      build.Refresh(evt.pointerClick.name);
    }
    else
    {
      if (build.isActiveAndEnabled && evt.pointerClick.name.Equals(build.current)) build.gameObject.SetActive(false);
      else build.gameObject.SetActive(true);
      build.Refresh(evt.pointerClick.name);
    }

  }

  private void ClickElectronic(PointerEventData evt)
  {
    if (build == null)
    {
      build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
      build.Refresh(evt.pointerClick.name);
    }
    else
    {
      if (build.isActiveAndEnabled && evt.pointerClick.name.Equals(build.current)) build.gameObject.SetActive(false);
      else build.gameObject.SetActive(true);
      build.Refresh(evt.pointerClick.name);
    }
  }

  private void ClickStation(PointerEventData evt)
  {
    if (build == null)
    {
      build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
      build.Refresh(evt.pointerClick.name);
    }
    else
    {
      if (build.isActiveAndEnabled && evt.pointerClick.name.Equals(build.current)) build.gameObject.SetActive(false);
      else build.gameObject.SetActive(true);
      build.Refresh(evt.pointerClick.name);
    }
  }

  private void ClickCook(PointerEventData evt)
  {
    if (build == null)
    {
      build = Managers.UI.MakeSubItem<UI_BuildPopup>(this.transform);
      build.Refresh(evt.pointerClick.name);
    }
    else
    {
      if (build.isActiveAndEnabled && evt.pointerClick.name.Equals(build.current)) build.gameObject.SetActive(false);
      else build.gameObject.SetActive(true);
      build.Refresh(evt.pointerClick.name);
    }
  }

  private void ClickPriority(PointerEventData evt)
  {
    Managers.UI.ShowPopupUI<UI_PriorityPopup>("UI_PriorityPopup");
  }

  private void Update()
  {
    PeriodText.text = $"Period\n{Managers.GameDay.Period}";
    PeriodMoon.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
  }
}
