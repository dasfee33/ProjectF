using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_Option : UI_Popup
{
  public BaseObject Owner;

  public enum Objects
  {
    ItemContent,
    JobButton,
  }

  public enum Buttons
  {
    ActButton,
    Job,
    LeftArrow,
    RightArrow,
  }

  public enum Texts
  {
    ActButtonText,
    DescText,

    JobButtonText,
    JobNumberText,
  }


  public System.Action Exit;
  public System.Action<Data.ConsumableItemData> ClickSomething;

  private Data.ConsumableItemData curData;
  private int jobNumber = 0;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindObjects(typeof(Objects));
    BindButtons(typeof(Buttons));
    BindTexts(typeof(Texts)); 


    Exit -= Cancel;
    Exit += Cancel;

    ClickSomething -= ShowActButton;
    ClickSomething += ShowActButton;

    GetButton((int)Buttons.ActButton).gameObject.BindEvent(ClickActButton, FUIEvent.Click);
    GetButton((int)Buttons.Job).gameObject.BindEvent(ClickJobButton, FUIEvent.Click);

    GetButton((int)Buttons.LeftArrow).gameObject.BindEvent((evt) => 
    {
      jobNumber = Mathf.Clamp(--jobNumber, 0, 99);
      Managers.Object.MakeItemNumberSet(curData.DataId, jobNumber);
      GetText((int)Texts.JobNumberText).text = jobNumber.ToString();
    }, FUIEvent.Click);
    GetButton((int)Buttons.RightArrow).gameObject.BindEvent((evt) =>
    {
      jobNumber = Mathf.Clamp(++jobNumber, 0, 99);
      Managers.Object.MakeItemNumberSet(curData.DataId, jobNumber);
      GetText((int)Texts.JobNumberText).text = jobNumber.ToString();
    }, FUIEvent.Click);


    return true;
  }

  public void SetInfo(BaseObject obj)
  {
    Owner = obj;
    var itemDict = Managers.Object.ItemStorage;
    var trans = GetObject((int)Objects.ItemContent).transform;
    switch (obj.ObjectType)
    {
      case FObjectType.Structure:
        var structure = obj as Structure;
        if(structure != null)
        {
          switch(structure.StructureSubType)
          {
            case FStructureSubType.PlowBowl:
            case FStructureSubType.Soil:
              foreach (var item in itemDict)
              {
                if(Managers.Data.ConsumableDic[item.Key].ItemSubType is FItemSubType.Seed)
                {
                  var data = Managers.Data.ConsumableDic[item.Key];
                  var optionItem = Managers.Resource.Instantiate("UI_OptionItem", trans);
                  var optionScr = optionItem.GetComponent<UI_OptionItem>();

                  optionScr.SetInfo(data, this);
                }
              }
              break;
            case FStructureSubType.Kitchen:
              var makeItemList = structure.data.makeItemid;
              foreach(var makeItem in makeItemList)
              {
                foreach(var item in itemDict)
                {
                  if(item.Key == makeItem)
                  {
                    var data = Managers.Data.ConsumableDic[item.Key];
                    var optionItem = Managers.Resource.Instantiate("UI_OptionItem", trans);
                    var optionScr = optionItem.GetComponent<UI_OptionItem>();

                    optionScr.SetInfo(data, this);
                    break;
                  }
                }
              }
              break;
          }
        }
        break;
    }
  }

  public void Cancel()
  {
    this.gameObject.SetActive(false);
    GetObject((int)Objects.ItemContent).DestroyChilds();
    GetButton((int)Buttons.ActButton).gameObject.SetActive(false);

    curData = null;
  }

  public void ShowActButton(Data.ConsumableItemData data)
  {
    curData = data;
    var itemDict = Managers.Object.ItemStorage;
    jobNumber = itemDict[data.DataId].makeItemNumber;
    ResetButton();

    switch (data.ItemSubType)
    {
      case FItemSubType.Seed:
        GetButton((int)Buttons.ActButton).gameObject.SetActive(true);
        GetText((int)Texts.ActButtonText).text = Managers.Game.GetText("PLOW_SOMETHING", data.Name);
        GetText((int)Texts.DescText).text = $"{data.DescirptionTextID}";
        break;
      case FItemSubType.Food:
        GetObject((int)Objects.JobButton).gameObject.SetActive(true);
        GetText((int)Texts.JobButtonText).text = "만들기"; //TODO
        break;
    }
    GetText((int)Texts.JobNumberText).text = jobNumber.ToString();

  }

  private void ResetButton()
  {
    GetButton((int)Buttons.ActButton).gameObject.SetActive(false);
    GetObject((int)Objects.JobButton).gameObject.SetActive(false);
  }

  public void ClickActButton(PointerEventData evt)
  {
    if(curData != null)
    {
      var toolBase = Managers.Map.Map.GetComponent<ToolBase>();

      switch(Owner)
      {
        case PlowBowl plowBowl:
          toolBase.SpawnBuildObject(curData, plowBowl.plantPort.position, true);
          break;
        case PlowSoil plowSoil:
          toolBase.SpawnBuildObject(curData, plowSoil.plantPort.position, true);
          break;
      }

      Cancel();

      //switch(curData.ItemSubType)
      //{
      //  case FItemSubType.Seed:
      //    var owner = Owner as PlowBowl;
      //    if(owner != null)
      //    {
      //      owner.plantSeed.Invoke(curData);
      //      Cancel();
      //    }
      //    break;
      //}
    }
  }

  public void ClickJobButton(PointerEventData evt)
  {
    if (curData != null)
    {
      switch(Owner)
      {
        case Kitchen kitchen:
          kitchen.makeOrder = jobNumber;
          break;
      }
    }
  }
}
