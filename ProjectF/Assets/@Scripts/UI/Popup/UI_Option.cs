using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Define;

public class UI_Option : UI_Popup
{
  public BaseObject Owner;

  public enum Objects
  {
    ItemContent,
  }

  public enum Buttons
  {
    ActButton,
  }

  public enum Texts
  {
    ActButtonText,
    DescText,
  }


  public System.Action Exit;
  public System.Action<Data.ConsumableItemData> ClickSomething;

  private Data.ConsumableItemData curData;

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

    return true;
  }

  public void SetInfo(BaseObject obj)
  {
    Owner = obj;
    switch(obj.ObjectType)
    {
      case FObjectType.Structure:
        var structure = obj as Structure;
        if(structure != null)
        {
          switch(structure.StructureSubType)
          {
            case FStructureSubType.PlowBowl:
              var itemDict = Managers.Object.ItemStorage;
              foreach(var item in itemDict)
              {
                if(Managers.Data.ConsumableDic[item.Key].ItemSubType is FItemSubType.Seed)
                {
                  var data = Managers.Data.ConsumableDic[item.Key];
                  var trans = GetObject((int)Objects.ItemContent).transform;
                  var seed = Managers.Resource.Instantiate("UI_OptionItem", trans);
                  var seedScr = seed.GetComponent<UI_OptionItem>();
                  if (seedScr != null)
                  {
                    seedScr.SetInfo(data);
                    seedScr.parent = this;
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
    if (!GetButton((int)Buttons.ActButton).gameObject.activeSelf)
      GetButton((int)Buttons.ActButton).gameObject.SetActive(true);

    switch(data.ItemSubType)
    {
      case FItemSubType.Seed:
        GetText((int)Texts.ActButtonText).text = $"{data.Name} 심기";
        GetText((int)Texts.DescText).text = $"{data.DescirptionTextID}";
        break;
    }
  }

  public void ClickActButton(PointerEventData evt)
  {
    if(curData != null)
    {
      switch(curData.ItemSubType)
      {
        case FItemSubType.Seed:
          var owner = Owner as PlowBowl;
          if(owner != null)
          {
            owner.plantSeed.Invoke(curData);
            Cancel();
          }
          break;
      }
    }
  }
}
