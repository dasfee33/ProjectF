using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_PriorityPopup : UI_Popup
{
  public enum Images
  {
    Face,
  }

  public enum Texts
  {
    Name,
  }

  public enum Objects
  {
    TopFixed,
    Hori,
    Content,
  }

  public enum Buttons
  {
    Exit,
  }

  private int jobLength;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    SetSafeArea(FSetUISafeArea.All);

    BindImages(typeof(Images));
    BindTexts(typeof(Texts));
    BindObjects(typeof(Objects));
    BindButtons(typeof(Buttons));

    jobLength = System.Enum.GetValues(typeof(FJob)).Length;

    GetButton((int)Buttons.Exit).gameObject.BindEvent(ClickExit, FUIEvent.Click);


    SetTopFixed();
    SetContents();

    return true;
  }

  private void ClickExit(PointerEventData evt)
  {
    Managers.UI.ClosePopupUI(this);
  }

  private void SetTopFixed()
  {
    
    GetImage((int)Images.Face).gameObject.SetActive(false);
    GetText((int)Texts.Name).text = $"Civil Settings"; // TODO FIXME

    var hori = GetObject((int)Objects.Hori);
    var top = GetObject((int)Objects.TopFixed);

    top.GetComponent<UI_PriorityPopupTop>().fix = true;

    for(int i = 1; i < jobLength; i++)
    {
      var obj = Managers.Resource.Instantiate("SelectJob");
      obj.transform.SetParent(hori.transform, true);
      var objScr = obj.GetComponent<UI_PriorityPopupSelectJob>();
      objScr.SetInfo((FJob)i);
    } 
  }

  private void SetContents()
  {
    var contentTrans = GetObject((int)Objects.Content).transform;
    var creatureList = Managers.Object.Creatures;

    foreach (var creature in creatureList)
    {
      var obj = Managers.Resource.Instantiate("Top");
      var objScr = obj.GetComponent<UI_PriorityPopupTop>();
      obj.transform.SetParent(contentTrans);
      obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

      objScr.data = creature.CreatureData;
      objScr.Owner = creature;
      objScr.SetInfo();
    }
  }
}
