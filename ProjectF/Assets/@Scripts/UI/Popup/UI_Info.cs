using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using static Define;
using static Util;

public class UI_Info : UI_Popup
{
  public enum Buttons
  {
    Exit,

    StatusTab,
    JobTab,
  }

  public enum Objects
  {
    infoContent,
    effectContent,
    descContent,
  }

  private GameObject infoContentDesc;
  private TextMeshProUGUI infoContentText;

  private GameObject effectContentDesc;
  private TextMeshProUGUI effectContentText;

  private GameObject descContentDesc;
  private TextMeshProUGUI descContentText;

  private UI_Option option;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindButtons(typeof(Buttons));
    BindObjects(typeof(Objects));

    GetButton((int)Buttons.Exit).gameObject.BindEvent(Cancel, FUIEvent.Click);

    FindChild(GetObject((int)Objects.infoContent), INFOTITLE, true).BindEvent(ClickInfoContent, FUIEvent.Click);
    infoContentDesc = FindChild(GetObject((int)Objects.infoContent), INFODESC, true);
    infoContentText = FindChild(GetObject((int)Objects.infoContent), INFOTEXT, true).GetComponent<TextMeshProUGUI>();

    FindChild(GetObject((int)Objects.effectContent), INFOTITLE, true).BindEvent(ClickEffectContent, FUIEvent.Click);
    effectContentDesc = FindChild(GetObject((int)Objects.effectContent), INFODESC, true);
    effectContentText = FindChild(GetObject((int)Objects.effectContent), INFOTEXT, true).GetComponent<TextMeshProUGUI>();

    FindChild(GetObject((int)Objects.descContent), INFOTITLE, true).BindEvent(ClickDescContent, FUIEvent.Click);
    descContentDesc = FindChild(GetObject((int)Objects.descContent), INFODESC, true);
    descContentText = FindChild(GetObject((int)Objects.descContent), INFOTEXT, true).GetComponent<TextMeshProUGUI>();

    return true;
  }

  private void Cancel(PointerEventData evt)
  {
    this.gameObject.SetActive(false);
    if (option != null) option.Exit.Invoke();
  }

  private void ClickInfoContent(PointerEventData evt)
  {
    var rect = infoContentText.GetComponent<RectTransform>();
    var defaultSize = rect.sizeDelta;

    if (!infoContentDesc.activeSelf)
    {
      infoContentDesc.SetActive(true);
      //rect.sizeDelta = new Vector2(defaultSize.x, 0f);
      //rect.DOSizeDelta(defaultSize, 1f).SetEase(Ease.OutQuad).OnComplete(() => infoContentDesc.SetActive(true));
    }
    else
    {
      infoContentDesc.SetActive(false);
      //var foldSizeDelta = new Vector2(defaultSize.x, 0f);
      //rect.DOSizeDelta(foldSizeDelta, 1f).SetEase(Ease.OutQuad).OnComplete(() => infoContentDesc.SetActive(false)); ;
    }
  }

  private void ClickEffectContent(PointerEventData evt)
  {

  }

  private void ClickDescContent(PointerEventData evt)
  {

  }

  public void SetInfo(BaseObject obj, UI_Option option)
  {
    Structure structure = obj as Structure;
    if(structure != null)
    {
      infoContentText.text = structure.data.DescriptionTextID;
    }
    if (option != null) this.option = option;
    
  }
}
