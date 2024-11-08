using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Define;

public class Toilet : Structure
{
  public override FStructureState StructureState
  {
    get { return base.StructureState; }
    set
    {
      if (structureState != value)
      {
        base.StructureState = value;
        switch (value)
        {
          case FStructureState.Idle:
            UpdateAITick = 0f;
            break;
          case FStructureState.WorkStart:
            //UpdateAITick = data.WorkTime;
            UpdateAITick = 0.8f;
            break;
          case FStructureState.Work:
            //UpdateAITick = 1f;
            break;
          case FStructureState.WorkEnd:
            UpdateAITick = 0f;
            break;
        }
      }
    }
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    StructureType = FStructureType.Pipe;
    StructureSubType = FStructureSubType.Toilet;


    StartCoroutine(CoUpdateAI());
    return true;
  }

  protected override void UpdateIdle()
  {
    onWorkSomeOne = false;
  }

  protected override void UpdateWorkStart()
  {
    // 일 시작상태로 1초 뒤에 다시 들어오면 일 하는 상태로 변경
    /* if (StructureState is FStructureState.WorkStart)*/ StructureState = FStructureState.Work;
  }

  protected override void UpdateOnWork()
  {
    if (!onWorkSomeOne)
    {
      UpdateAITick = data.WorkTime;
      Worker.SpriteRenderer.DOFade(0, 0.8f).OnComplete(() =>
      {
        onWorkSomeOne = true;
      });
    }
    else
    {
      //틱 뒤에 다시 들어오면 일 끝남처리
      StructureState = FStructureState.WorkEnd;
    }
  }

  protected override void UpdateWorkEnd()
  {
    Worker.SpriteRenderer.DOFade(1, 0.8f).OnComplete(() =>
    {
      Worker.Target = null;
      Worker.ppSystem.target = null;
      Worker.SetOrAddJobPriority(workableJob, 0, true);
      Worker = null;

      StructureState = FStructureState.Idle;
      onWorkSomeOne = false;
    });
    
  }
}
