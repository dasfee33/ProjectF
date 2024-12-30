using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public static class Extension
{
  public static object ReturnProperty<T>(this IEnumerable<T> list, string property, int id) where T: class
  {
    foreach(var item in list)
    {
      var param = item.GetType().GetProperty(property);
      if (param != null)
      {
        var paramValue = param.GetValue(item);
        if((int)paramValue == id)
          return item;
      }
    }

    return null;
  }

  public static object ReturnProperty<T>(this IEnumerable<T> list, string property, string label) where T : class
  {
    foreach (var item in list)
    {
      var param = item.GetType().GetProperty(property);
      if (param != null)
      {
        var paramValue = param.GetValue(item);
        if (paramValue.ToString().Equals(label))
          return item;
      }
    }

    return null;
  }

  public static bool CompareIdProperty<T>(this IEnumerable<T> list, string property, int id) where T: class
  {
    foreach(var item in list)
    {
      var param = item.GetType().GetProperty(property);
      if(param != null)
      {
        var paramValue = param.GetValue(item);
        if ((int)paramValue == id)
          return true;
      }
    }
    return false;
  }

  public static bool CompareIabelProperty<T>(this IEnumerable<T> list, string property, string label) where T : class
  {
    foreach (var item in list)
    {
      var param = item.GetType().GetProperty(property);
      if (param != null)
      {
        var paramValue = param.GetValue(item);
        if (paramValue.ToString().Equals(label))
          return true;
      }
    }
    return false;
  }

  public static bool ContainsKey<T>(this IEnumerable<T> list, string property, string label = null) where T : class
  {
    foreach (var item in list)
    {
      var param = item.GetType().GetProperty(property);
      if (param != null)
      {
        if (!string.IsNullOrEmpty(label))
        {
          var paramValue = param.GetValue(item);
          if (paramValue.ToString().Equals(label))
            return true;
        }
        else return true;
      }
    }
    return false;
  }

  public static T GetOrAddComponent<T>(this GameObject go) where T : Component
  {
    return Util.GetOrAddComponent<T>(go);
  }

  public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, Define.FUIEvent type = Define.FUIEvent.Click)
  {
    UI_Base.BindEvent(go, action, type);
  }

  public static bool IsValid(this GameObject go)
  {
    return go != null && go.activeSelf;
  }

  public static bool IsValid(this BaseObject bo)
  {
    if (bo == null || bo.isActiveAndEnabled == false)
      return false;

    Creature creature = bo as Creature;
    if (creature != null)
      return creature.CreatureState != Define.FCreatureState.Dead;

    return true;
  }

  public static void DestroyChilds(this GameObject go)
  {
    foreach (Transform child in go.transform)
      Managers.Resource.Destroy(child.gameObject);
  }

  public static void DestroyChilds<T>(this ICollection<T> collection) where T : Object
  {
    foreach (var child in collection)
    {
      if(child is not null)
      {
        if (child is GameObject gameObject) Object.Destroy(gameObject);
        else if (child is Component component) Object.Destroy(component.gameObject);
        else Object.Destroy(child);
      }
    }

    collection.Clear();
  }

  public static void TranslateEx(this Transform transform, Vector3 dir)
  {
    BaseObject bo = transform.gameObject.GetComponent<BaseObject>();
    if (bo != null) bo.TranslateEx(dir);
  }

  public static void Shuffle<T>(this IList<T> list)
  {
    int n = list.Count;

    while (n > 1)
    {
      n--;
      int k = UnityEngine.Random.Range(0, n + 1);
      (list[k], list[n]) = (list[n], list[k]); //swap
    }
  }
}
