using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CookTable : Structure
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

    StructureType = FStructureType.Furniture;
    StructureSubType = FStructureSubType.EatingTable;
    spriteRenderer.sortingOrder = 20;

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
    if(!onWorkSomeOne)
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
    Worker.SupplyFromHaveListCalories();

    Worker.SetOrAddJobPriority(workableJob, 0, true);
    Managers.Event.MoodStable(Worker, workableJob);
    Worker.ResetJob();

    StructureState = FStructureState.Idle;
    onWorkSomeOne = false;

  }
}
