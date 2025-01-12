using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_PriorityPopupTop : UI_Base
{
  public Data.CreatureData data;
  public Creature Owner;

  public enum Images
  {
    Face,
  }

  public enum Objects
  {
    Hori,
  }

  public enum Texts
  {
    Name,
  }

  public bool fix = false;
  private int jobLength;
  private Transform hori;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindImages(typeof(Images));
    BindObjects(typeof(Objects));
    BindTexts(typeof(Texts));

    hori = GetObject((int)Objects.Hori).transform;

    jobLength = System.Enum.GetValues(typeof(FJob)).Length;

    return true;
  }

  //사진 / 이름 설정
  public void SetInfo()
  {
    GetImage((int)Images.Face).sprite = Managers.Resource.Load<Sprite>(data.Head);
    GetText((int)Texts.Name).text = Managers.Game.GetText(data.Name);

    for (int i = 1; i < jobLength; i++)
    {
      var obj = Managers.Resource.Instantiate("SelectJobButton");
      obj.transform.SetParent(hori, true);
      var objScr = obj.GetComponent<UI_PriorityPopupSelectJobButton>();
      objScr._owner = Owner;
      objScr.SetInfo((FJob)i);
    }
  }
}
