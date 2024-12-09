using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static Util;

public class UI_BuildPopup : UI_Popup
{
  enum Objects
  {
    Content,
  }

  enum Buttons
  {
    Exit,

    Grid,
    Vert,
  }

  public string current;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindObjects(typeof(Objects));
    BindButtons(typeof(Buttons));

    GetButton((int)Buttons.Exit).gameObject.BindEvent(Exit);

    //Managers.FInput.OnTouch -= SelectAnother;
    //Managers.FInput.OnTouch += SelectAnother;

    //FIXME TEMP
    if (Managers.Object.PossStations.Count <= 0)
    {
      Managers.Object.PossStations.Add(STRUCTURE_STATION_NORMAL);
      Managers.Object.PossFurnitures.Add(STRUCTURE_BED_NORMAL);
      Managers.Object.PossFurnitures.Add(STRUCTURE_CHEST_NORMAL);
      Managers.Object.PossPipes.Add(STRUCTURE_TOILET_NORMAL);
      Managers.Object.PossCooks.Add(STRUCTURE_PLOWBOWL_NORMAL);
      Managers.Object.PossCooks.Add(STRUCTURE_COOKTABLE_NORMAL);
    }

    return true;
  }

  public void Refresh(string name)
  {
    if (string.IsNullOrEmpty(name)) return;
    if (!name.Equals(current) || string.IsNullOrEmpty(current))
    {
      current = name;
      ClearList();
    }
    else return;

    switch(name)
    {
      case "Base":
        foreach (var bases in Managers.Object.PossBases)
        {
          var obj = Managers.Resource.Instantiate("UI_BuildPopup_Item");
          var objScr = obj.GetComponent<UI_BuildPopup_Item>();
          obj.transform.SetParent(GetObject((int)Objects.Content).transform, true);
          objScr.SetInfo(bases);
        }
        break;
      case "Furniture":
        foreach (var furni in Managers.Object.PossFurnitures)
        {
          var obj = Managers.Resource.Instantiate("UI_BuildPopup_Item");
          var objScr = obj.GetComponent<UI_BuildPopup_Item>();
          obj.transform.SetParent(GetObject((int)Objects.Content).transform, true);
          objScr.SetInfo(furni);
        }
        break;
      case "Pipe":
        foreach (var pipe in Managers.Object.PossPipes)
        {
          var obj = Managers.Resource.Instantiate("UI_BuildPopup_Item");
          var objScr = obj.GetComponent<UI_BuildPopup_Item>();
          obj.transform.SetParent(GetObject((int)Objects.Content).transform, true);
          objScr.SetInfo(pipe);
        }
        break;
      case "Electronic":
        foreach (var electro in Managers.Object.PossElectronics)
        {
          var obj = Managers.Resource.Instantiate("UI_BuildPopup_Item");
          var objScr = obj.GetComponent<UI_BuildPopup_Item>();
          obj.transform.SetParent(GetObject((int)Objects.Content).transform, true);
          objScr.SetInfo(electro);
        }
        break;
      case "Station":
        foreach(var station in Managers.Object.PossStations)
        {
          var obj = Managers.Resource.Instantiate("UI_BuildPopup_Item");
          var objScr = obj.GetComponent<UI_BuildPopup_Item>();
          obj.transform.SetParent(GetObject((int)Objects.Content).transform, true);
          objScr.SetInfo(station);
        }
        break;
      case "Cook":
        foreach (var station in Managers.Object.PossCooks)
        {
          var obj = Managers.Resource.Instantiate("UI_BuildPopup_Item");
          var objScr = obj.GetComponent<UI_BuildPopup_Item>();
          obj.transform.SetParent(GetObject((int)Objects.Content).transform, true);
          objScr.SetInfo(station);
        }
        break;
    }
  }

  public void ClearList()
  {
    var obj = GetObject((int)Objects.Content);
    var len = obj.transform.childCount;

    for (int i = 0; i < len; i++)
    {
      Managers.Resource.Destroy(obj.transform.GetChild(i).gameObject);
    }
  }

  //private void SelectAnother(Vector3 pos)
  //{
  //  //FIXME
  //  if (!this.gameObject.activeSelf && EventSystem.current.IsPointerOverGameObject() == false)
  //  {
  //    Debug.Log($"{this.name} : another point clicked");
  //    Exit();
  //  }
  //}

  private void Exit(PointerEventData evt = null)
  {
    this.gameObject.SetActive(false);
  }
}
