using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_CreatureActionEvent : UI_Base
{
  public enum Texts 
  {
    EventText,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindTexts(typeof(Texts));

    return true;
  }
  
  public void SetInfo(string notice)
  {
    GetText((int)Texts.EventText).text = notice;
    var rect = this.GetComponent<RectTransform>();
    rect.DOAnchorPosY(0.8f, 1f).SetEase(Ease.OutQuint).OnComplete(() =>
    {
      Managers.Resource.Destroy(this.gameObject);
    });
  }
}
