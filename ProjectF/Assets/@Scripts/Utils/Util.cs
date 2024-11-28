using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public static class Util
{
  public static long OneGB = 1000000000;
  public static long OneMB = 1000000;
  public static long OneKB = 1000;

  public static bool IsNetworkValid()
  {
    return Application.internetReachability != NetworkReachability.NotReachable;
  }

  public static bool IsDiskSpaceEnough(long requiredSize)
  {
    return Caching.defaultCache.spaceFree >= requiredSize;
  }

  public static FSizeUnits GetProperByteUnit(long byteSize)
  {
    if (byteSize >= OneGB) return FSizeUnits.GB;
    else if(byteSize >= OneMB) return FSizeUnits.MB;
    else if(byteSize >= OneKB) return FSizeUnits.KB;
    return FSizeUnits.Byte;
  }

  public static long ConvertByteByUnit(long byteSize, FSizeUnits unit)
  {
    return (long)((byteSize / (double)System.Math.Pow(1024, (long)unit)));
  }

  public static string GetConvertedByteString(long byteSize, FSizeUnits unit, bool appendUnit = true)
  {
    string unitStr = appendUnit ? unit.ToString() : string.Empty;
    return $"{ConvertByteByUnit(byteSize, unit).ToString("0.00")}{unitStr}";
  }

  public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
  {
    T component = go.GetComponent<T>();
    if (component == null)
    {
      component = go.GetComponentInChildren<T>();
      if (component == null)
      {
        component = go.AddComponent<T>();
      }
    }
    return component;
  }

  public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
  {
    Transform transform = FindChild<Transform>(go, name, recursive);
    if (transform == null)
      return null;

    return transform.gameObject;
  }

  public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
  {
    if (go == null)
      return null;

    if (recursive == false)
    {
      for (int i = 0; i < go.transform.childCount; i++)
      {
        Transform transform = go.transform.GetChild(i);
        if (string.IsNullOrEmpty(name) || transform.name == name)
        {
          T component = transform.GetComponent<T>();
          if (component != null)
            return component;
        }
      }
    }
    else
    {
      foreach (T component in go.GetComponentsInChildren<T>(true))
      {
        if (string.IsNullOrEmpty(name) || component.name == name)
          return component;
      }
    }

    return null;
  }

  public static T ParseEnum<T>(string value)
  {
    return (T)Enum.Parse(typeof(T), value, true);
  }

  public static Color HexToColor(string color)
  {
    if (color.Contains("#") == false)
      color = $"#{color}";

    ColorUtility.TryParseHtmlString(color, out Color parsedColor);

    return parsedColor;
  }

  public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
  {
    float totalWeight = sequence.Sum(weightSelector);

    double itemWeightIndex = new System.Random().NextDouble() * totalWeight;
    float currentWeightIndex = 0;

    foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
    {
      currentWeightIndex += item.Weight;

      // If we've hit or passed the weight we are after for this item then it's the one we want....
      if (currentWeightIndex >= itemWeightIndex)
        return item.Value;

    }

    return default(T);
  }
}
