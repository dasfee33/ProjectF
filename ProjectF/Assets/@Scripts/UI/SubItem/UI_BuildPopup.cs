using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_BuildPopup : UI_Popup
{
  enum Buttons
  {
    Exit,

    Grid,
    Vert,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindButtons(typeof(Buttons));

    GetButton((int)Buttons.Exit).gameObject.BindEvent(Exit);

    return true;
  }

  public void Refresh()
  {

  }

  private void Update()
  {
    if (EventSystem.current.IsPointerOverGameObject() == false)
    {
      Debug.Log($"{this.name} : another point clicked");
      Exit();
    }
      
  }

  private void Exit(PointerEventData evt = null)
  {
    this.gameObject.SetActive(false);
  }
}
