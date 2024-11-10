using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Base : InitBase
{
  protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
  protected RectTransform rect;
  protected Canvas canvas;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    rect = this.GetComponent<RectTransform>();
    canvas = this.GetComponentInParent<Canvas>();
    if (canvas == null) canvas = this.transform.root.GetComponent<Canvas>();

    return true;
  }

  protected virtual void SetSafeArea(FSetUISafeArea type)
  {
    var safeArea = Screen.safeArea;
    var width = Screen.width;
    var height = Screen.height;

    //float canvasScaleFactor = canvas.scaleFactor;

    rect.anchorMin = new Vector2
    (
      type.HasFlag(FSetUISafeArea.Left) ? (safeArea.xMin / width) /*/ canvasScaleFactor*/ : 0f,
      type.HasFlag(FSetUISafeArea.Bottom) ? (safeArea.yMin / height) /*/ canvasScaleFactor*/ : 0f
    );

    rect.anchorMax = new Vector2
    (
      type.HasFlag(FSetUISafeArea.Right) ? (safeArea.xMax / width) /*/ canvasScaleFactor*/ : 1f,
      type.HasFlag(FSetUISafeArea.Top) ? (safeArea.yMax / height) /*/ canvasScaleFactor*/ : 1f
    );
  }

  protected void Bind<T>(Type type) where T : UnityEngine.Object
  {
    string[] names = Enum.GetNames(type);
    UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
    _objects.Add(typeof(T), objects);

    for (int i = 0; i < names.Length; i++)
    {
      if (typeof(T) == typeof(GameObject))
        objects[i] = Util.FindChild(gameObject, names[i], true);
      else
        objects[i] = Util.FindChild<T>(gameObject, names[i], true);

      if (objects[i] == null)
        Debug.Log($"Failed to bind({names[i]})");
    }
  }

  protected void BindObjects(Type type) { Bind<GameObject>(type); }
  protected void BindImages(Type type) { Bind<Image>(type); }
  protected void BindTexts(Type type) { Bind<TextMeshProUGUI>(type); }
  protected void BindButtons(Type type) { Bind<Button>(type); }
  protected void BindToggles(Type type) { Bind<Toggle>(type); }
  protected void BindSliders(Type type) { Bind<Slider>(type); }

  protected T Get<T>(int idx) where T : UnityEngine.Object
  {
    UnityEngine.Object[] objects = null;
    if (_objects.TryGetValue(typeof(T), out objects) == false)
      return null;
    return objects[idx] as T;
  }

  protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
  protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
  protected Button GetButton(int idx) { return Get<Button>(idx); }
  protected Image GetImage(int idx) { return Get<Image>(idx); }
  protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }
  protected Slider GetSlider(int idx) { return Get<Slider>(idx); }

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
