using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_Option : UI_Popup
{
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
  }


  public System.Action Exit;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindObjects(typeof(Objects));
    BindButtons(typeof(Buttons));
    BindTexts(typeof(Texts)); 


    Exit -= Cancel;
    Exit += Cancel;

    return true;
  }

  public void SetInfo(BaseObject obj)
  {
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
                if(Managers.Data.ItemDic[item.Key].ItemSubType is FItemSubType.Seed)
                {
                  var data = Managers.Data.ItemDic[item.Key];
                  var trans = GetObject((int)Objects.ItemContent).transform;
                  var seed = Managers.Resource.Instantiate("UI_OptionItem", trans);
                  var seedScr = seed.GetComponent<UI_OptionItem>();
                  if (seedScr != null) seedScr.SetInfo(data);
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
  }
}
