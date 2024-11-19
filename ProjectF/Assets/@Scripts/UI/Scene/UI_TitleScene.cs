using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_TitleScene : UI_Scene
{
  enum GameObjects
  {
    Background,
    Startarea,
  }

  enum Texts
  {
    Gamestart,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindObjects(typeof(GameObjects));
    BindTexts(typeof(Texts));

    GetObject((int)GameObjects.Startarea).BindEvent((evt) =>
    {
      Debug.Log("Change Scene");
      Managers.Scene.LoadScene(FScene.GameScene);
    });

    StartLoadAssets();

    return true;
  }

  private void StartLoadAssets()
  {
    Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
    {
      Debug.Log($"{key} {count}/{totalCount}");

      if (count == totalCount)
      {
        //데이터 초기화
        Managers.Data.Init();

        if(Managers.Game.LoadGame() == false)
        {
          Managers.Game.InitGame();
          Managers.Game.SaveGame();
        }

        //Managers.Scene.LoadScene(//TODO)
      }
    });
  }
}
