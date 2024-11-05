using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_Game : UI_Scene
{
  enum Buttons
  {
    Image,
    Image2
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindButtons(typeof(Buttons));

    GetButton((int)Buttons.Image).gameObject.BindEvent(Test, FUIEvent.Click);
    GetButton((int)Buttons.Image2).gameObject.BindEvent(Test2, FUIEvent.Click);
    

    return true;
  }

  public void Test(PointerEventData evt)
  {
    Time.timeScale *= 2;
  }

  public void Test2(PointerEventData evt)
  {
    Time.timeScale /= 2;
  }
}
