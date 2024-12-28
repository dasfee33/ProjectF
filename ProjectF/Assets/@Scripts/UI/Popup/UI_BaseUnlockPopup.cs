using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BaseUnlockPopup : UI_Popup
{
  private Data.ResearchData nextResearchData;

  public enum Objects
  {
    UnlockItem,
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

    return true;
  }

  public void SetInfo()
  {
    var baseLevel = Managers.Game.SaveData.realGameData.baseLevel;
    GetText((int)Texts.BaseLevelText).text = $"현재 마을 발전도 : {baseLevel}";
    GetText((int)Texts.BaseNextLevelUnlock).text = $"발전도  : {baseLevel + 1}에 해금!";

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
  }

  public void OnDisable()
  {
    GetObject((int)Objects.UnlockItem).DestroyChilds();
  }
}
