using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Define;

public class UI_MapSelectScene : UI_Scene
{
  public enum Buttons
  {
    GoButton,
  }

  public enum Texts
  {

  }

  public override bool Init()
  {
    if (base.Init() == false) return false;
    SetSafeArea(FSetUISafeArea.All);

    BindButtons(typeof(Buttons));
    BindTexts(typeof(Texts));

    GetButton((int)Buttons.GoButton).gameObject.BindEvent(ClickGoButton, FUIEvent.Click);

    return true;
  }

  public void ClickGoButton(PointerEventData evt)
  {
    Managers.Scene.LoadScene(FScene.GameScene);
  }
}
