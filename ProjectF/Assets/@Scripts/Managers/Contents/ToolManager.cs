using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolManager
{
  public static void BindEvent(GameObject go, Action<PointerEventData> action = null, Define.FUIEvent type = Define.FUIEvent.Click)
  {
    UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

    switch (type)
    {
      case Define.FUIEvent.Click:
        evt.OnClickHandler -= action;
        evt.OnClickHandler += action;
        break;
      case Define.FUIEvent.PointerDown:
        evt.OnPointerDownHandler -= action;
        evt.OnPointerDownHandler += action;
        break;
      case Define.FUIEvent.PointerUp:
        evt.OnPointerUpHandler -= action;
        evt.OnPointerUpHandler += action;
        break;
      case Define.FUIEvent.Drag:
        evt.OnDragHandler -= action;
        evt.OnDragHandler += action;
        break;
    }
  }
}
