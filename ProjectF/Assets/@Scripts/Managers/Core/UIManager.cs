using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager
{
  private int _order = 10;

  private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

  private UI_Scene _sceneUI = null;
  public UI_Scene SceneUI
  {
    set { _sceneUI = value; }
    get { return _sceneUI; }
  }

  public GameObject Root
  {
    get
    {
      GameObject root = GameObject.Find("@UI_Root");
      if (root == null)
        root = new GameObject { name = "@UI_Root" };
      return root;
    }
  }

  public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
  {
    Canvas canvas = go.GetComponent<Canvas>();
    if(canvas == null)
    {
      canvas = go.transform.root.GetComponent<Canvas>();
      if (canvas != null)
      {
        SetCanvas(canvas, sort, sortOrder);
      }
      else return;
    }
    else
    {
      SetCanvas(canvas, sort, sortOrder);
    }

    //Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
    //if (canvas == null)
    //{
    //  canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    //  canvas.overrideSorting = true;
    //}

    //CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
    
  }

  private void SetCanvas(Canvas canvas, bool sort, int sortOrder = 0)
  {
    canvas.overrideSorting = true;

    CanvasScaler cs = canvas.GetComponent<CanvasScaler>();
    if (cs != null)
    {
      cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
      cs.referenceResolution = new Vector2(1920, 1080);
    }

    canvas.GetOrAddComponent<GraphicRaycaster>();

    if (sort)
    {
      canvas.sortingOrder = _order;
      _order++;
    }
    else
    {
      canvas.sortingOrder = sortOrder;
    }
  }

  public T GetSceneUI<T>() where T : UI_Base
  {
    return _sceneUI as T;
  }

  public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
  {
    if (string.IsNullOrEmpty(name))
      name = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate($"{name}");
    if (parent != null)
      go.transform.SetParent(parent);

    Canvas canvas = go.GetOrAddComponent<Canvas>();
    canvas.renderMode = RenderMode.WorldSpace;
    canvas.worldCamera = Camera.main;

    return Util.GetOrAddComponent<T>(go);
  }

  public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base
  {
    if (string.IsNullOrEmpty(name))
      name = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate(name, parent, pooling);
    if(go == null) { return null; }
    go.transform.SetParent(parent, false);

    return Util.GetOrAddComponent<T>(go);
  }

  public T ShowBaseUI<T>(string name = null) where T : UI_Base
  {
    if (string.IsNullOrEmpty(name))
      name = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate(name);
    T baseUI = Util.GetOrAddComponent<T>(go);

    go.transform.SetParent(Root.transform);

    return baseUI;
  }

  public T ShowSceneUI<T>(string name = null) where T : UI_Scene
  {
    if (string.IsNullOrEmpty(name))
      name = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate(name);
    if (go == null) { Debug.LogWarning($"SceneUI {name} is not Exist!!"); return null; }
    T sceneUI = Util.GetOrAddComponent<T>(go);
    _sceneUI = sceneUI;

    go.transform.SetParent(Root.transform);

    return sceneUI;
  }

  public T ShowPopupUI<T>(string name = null) where T : UI_Popup
  {
    if (string.IsNullOrEmpty(name))
      name = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate(name);
    if (go == null) { Debug.LogWarning($"PopupUI {name} is not Exist!!"); return null; }
    T popup = Util.GetOrAddComponent<T>(go);
    _popupStack.Push(popup);

    go.transform.SetParent(Root.transform);

    return popup;
  }

  public void ClosePopupUI(UI_Popup popup)
  {
    if (_popupStack.Count == 0)
      return;

    if (_popupStack.Peek() != popup)
    {
      Debug.Log("Close Popup Failed!");
      return;
    }

    ClosePopupUI();
  }

  public void ClosePopupUI()
  {
    if (_popupStack.Count == 0)
      return;

    UI_Popup popup = _popupStack.Pop();
    //popup.transform.parent.gameObject.SetActive(false);
    Managers.Resource.Destroy(popup.transform.parent.gameObject);
    _order--;
  }

  public void CloseAllPopupUI()
  {
    while (_popupStack.Count > 0)
      ClosePopupUI();
  }

  public int GetPopupCount()
  {
    return _popupStack.Count;
  }

  public void Clear()
  {
    CloseAllPopupUI();
    _sceneUI = null;
  }
}
