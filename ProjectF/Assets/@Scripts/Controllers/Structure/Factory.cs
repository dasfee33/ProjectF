using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Factory : Structure
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
            UpdateAITick = 0f;
            break;
          case FStructureState.Work:
            UpdateAITick = data.WorkTime;
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

    StructureType = FStructureType.Base;
    StructureSubType = FStructureSubType.Factory;


    StartCoroutine(CoUpdateAI());
    return true;
  }

  protected override void UpdateIdle()
  {
    onWorkSomeOne = false;
  }

  protected override void UpdateWorkStart()
  {
    StructureState = FStructureState.Work;
  }

  protected override void UpdateOnWork()
  {
    if (!onWorkSomeOne)
    {
      UpdateAITick = data.WorkTime;
      onWorkSomeOne = true;
    }
    else
    {
      StructureState = FStructureState.WorkEnd;
    }


  }

  protected override void UpdateWorkEnd()
  {
    Worker.ResetJob();

    StructureState = FStructureState.Idle;
    onWorkSomeOne = false;

    //TODO
  }
}
