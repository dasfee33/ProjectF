using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FSceneManager
{
  public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

  public void LoadScene(Define.FScene type)
  {
    SceneManager.LoadScene(GetSceneName(type));

  }

  private string GetSceneName(Define.FScene type)
  {
    string name = System.Enum.GetName(typeof(Define.FScene), type);
    return name;
  }

  public void Clear()
  {
    //CurrentScene.Clear();
  }

}
