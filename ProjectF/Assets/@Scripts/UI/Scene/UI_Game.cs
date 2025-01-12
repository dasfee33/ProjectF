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
    UI_BaseUnlockPopup,
  }

  enum Buttons
  {
    //Dev
    Fast,
    Slow,
    SaveData,
    UpdateData,
    ResetCreature,

    Base,
    Furniture,
    Pipe,
    Electronic,
    Station,
    Cook,

    Priority,

    PeriodMoon,
  }
  enum Images
  {
    PeriodMoon,
  }
  enum Texts
  {
    PeriodText,
    Notice,
  }

  private Image PeriodMoon;
  private TextMeshProUGUI PeriodText;
  private float rotationSpeed;
  private UI_BuildPopup build;
  private GameObject baseUnlock;

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
    GetButton((int)Buttons.ResetCreature).gameObject.BindEvent(ResetCreature, FUIEvent.Click);

    GetButton((int)Buttons.Base).gameObject.BindEvent(ClickBase, FUIEvent.Click);
    GetButton((int)Buttons.Furniture).gameObject.BindEvent(ClickFurniture, FUIEvent.Click);
    GetButton((int)Buttons.Pipe).gameObject.BindEvent(ClickPipe, FUIEvent.Click);
    GetButton((int)Buttons.Electronic).gameObject.BindEvent(ClickElectronic, FUIEvent.Click);
    GetButton((int)Buttons.Station).gameObject.BindEvent(ClickStation, FUIEvent.Click);
    GetButton((int)Buttons.Cook).gameObject.BindEvent(ClickCook, FUIEvent.Click);

    GetButton((int)Buttons.Priority).gameObject.BindEvent(ClickPriority, FUIEvent.Click);
    GetButton((int)Buttons.PeriodMoon).gameObject.BindEvent(ClickPeriodMoon, FUIEvent.Click);

    PeriodMoon = GetImage((int)Images.PeriodMoon);
    PeriodText = GetText((int)Texts.PeriodText);

    Managers.FInput.touchObject -= CallbackSomething;
    Managers.FInput.touchObject += CallbackSomething;

    Managers.Event.notice -= NoticeAlarm;
    Managers.Event.notice += NoticeAlarm;

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

  private void ResetCreature(PointerEventData evt)
  {
    foreach(var creature in Managers.Object.Creatures)
    {
      creature.ResetJob();
    }
  }

  private Coroutine coNotice;
  public void NoticeAlarm(string notice)
  {
    var text = GetText((int)Texts.Notice);
    text.gameObject.SetActive(true);
    text.text = notice;

    if (coNotice != null) StopCoroutine(coNotice);
    text.color = Color.white;

    coNotice = StartCoroutine(TextFadeOut(text));
  }

  private IEnumerator TextFadeOut(TextMeshProUGUI notice)
  {
    float alpha = 1f;

    while (alpha > 0f)
    {
      notice.color = new Vector4(1, 1, 1, alpha);
      alpha -= (0.1f * 5 * Time.deltaTime);
      yield return null;
    }

    notice.gameObject.SetActive(false);
  }

  public void CallbackSomething(BaseObject obj)
  {
    var info = GetObject((int)Objects.UI_Info).gameObject.GetComponent<UI_Info>();
    info.gameObject.SetActive(true);

    if (obj.Option)
    {
      switch(obj.ObjectType)
      {
        case FObjectType.Structure:
          var structure = obj as Structure;
          switch(structure.StructureSubType)
          {
            case FStructureSubType.PlowBowl:
              var option = GetObject((int)Objects.UI_Option).gameObject.GetComponent<UI_PlowOption>();
              option.gameObject.SetActive(true);
              option.SetInfo(obj);
              info.SetInfo(obj, option);
              break;
          }
          break;
      }



    }

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

  private void ClickPeriodMoon(PointerEventData evt)
  {
    if (baseUnlock is null)
    {
      baseUnlock = GetObject((int)Objects.UI_BaseUnlockPopup);
      baseUnlock.SetActive(true);
      baseUnlock.GetComponent<UI_BaseUnlockPopup>().SetInfo();
    }
    else
    {
      baseUnlock.SetActive(false);
      baseUnlock = null;
    }

  }

  private void Update()
  {
    PeriodText.text = $"Period\n{Managers.GameDay.Period}";
    PeriodMoon.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
  }
}
