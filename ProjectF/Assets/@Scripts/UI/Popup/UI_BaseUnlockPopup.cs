using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_BaseUnlockPopup : UI_Popup
{
  private Data.ResearchData nextResearchData;

  public enum Objects
  {
    UnlockItem,
    InfoBoard,
  }

  public enum Buttons
  {
    ItemButton,
    UnlockButton,
  }

  public enum Texts
  {
    BaseInfoText, //Local Language 연결필요!
    BaseLevelText,
    BaseNextLevelUnlock,

    UnlockText,

  }

  public enum Sliders
  {
    NeedSlider,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindObjects(typeof(Objects));
    BindButtons(typeof(Buttons));
    BindTexts(typeof(Texts));
    BindSliders(typeof(Sliders));

    GetButton((int)Buttons.UnlockButton).gameObject.BindEvent(ClickUnlockButton, FUIEvent.Click);

    return true;
  }

  public void ClickUnlockButton(PointerEventData evt)
  {
    Managers.Game.SaveData.realGameData.baseLevel += 1;
    ResetUI();
    SetInfo();
  }

  private List<UI_BaseUnlockPopupNeedSlider> sliderList = new List<UI_BaseUnlockPopupNeedSlider>();

  public void SetInfo()
  {
    var baseLevel = Managers.Game.SaveData.realGameData.baseLevel;
    GetText((int)Texts.BaseLevelText).text = Managers.Game.GetText("CUR_TOWNEVOLUTION", baseLevel);
    GetText((int)Texts.BaseNextLevelUnlock).text = Managers.Game.GetText("TOWNEVOLUTION", baseLevel + 1);

    foreach(var data in Managers.Data.ResearchDic)
    {
      if(data.Value.Level == baseLevel + 1)
      {
        nextResearchData = Managers.Data.ResearchDic[data.Key];
        break;
      }
    }

    foreach(var nextReward in nextResearchData.Reward)
    {
      var reward = Managers.Data.StructDic[nextReward];
      var objTrans = GetObject((int)Objects.UnlockItem).transform;
      var subItem = Managers.UI.MakeSubItem<UI_BaseUnlockPopupItemButton>(objTrans, "ItemButton");
      subItem.SetInfo(reward.Sprite, nextReward);
    }

    var researchDic = Managers.Game.SaveData.realGameData.researchDic;
    int researchLevel = 1;
    foreach (var nextGauge in nextResearchData.NeedToUnlock)
    {
      var objTrans = GetObject((int)Objects.InfoBoard).transform;
      var subSlider = Managers.UI.MakeSubItem<UI_BaseUnlockPopupNeedSlider>(objTrans, "NeedSlider");
      sliderList.Add(subSlider);

      var researchValue = 0;
      if (researchDic.TryGetValue(researchLevel, out var v))
        researchValue = v;

      subSlider.SetInfo(researchLevel, researchValue, nextGauge);
      researchLevel++;
    }

    bool sliderCheck = sliderList.All(obj => obj.slider.maxValue / obj.slider.value == 1);

    var unlockButton = GetButton((int)Buttons.UnlockButton);

    if (sliderCheck) unlockButton.gameObject.SetActive(true);
    else unlockButton.gameObject.SetActive(false);

  }

  private void ResetUI()
  {
    GetObject((int)Objects.UnlockItem).DestroyChilds();
    sliderList.DestroyChilds();
  }

  public void OnDisable()
  {
    ResetUI();
  }
}
