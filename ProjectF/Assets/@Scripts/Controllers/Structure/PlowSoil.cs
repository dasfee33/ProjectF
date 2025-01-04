using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlowSoil : Structure
{
  public Transform plantPort;

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

    StructureType = FStructureType.Cook;
    StructureSubType = FStructureSubType.Soil;

    StartCoroutine(CoUpdateAI());
    return true;
  }

  protected override void UpdateIdle()
  {
    onWorkSomeOne = false;
  }

  protected override void UpdateWorkStart()
  {
    onWorkSomeOne = true;
    StructureState = FStructureState.Work;
  }

  protected override void UpdateOnWork()
  {
    if (onWorkSomeOne)
    {
      StructureState = FStructureState.WorkEnd;
    }


  }

  protected override void UpdateWorkEnd()
  {
    Worker.jobSystem.target = null;
    Worker.Target = null;
    Worker = null;

    StructureState = FStructureState.Idle;
    onWorkSomeOne = false;

    //TODO
  }
}
