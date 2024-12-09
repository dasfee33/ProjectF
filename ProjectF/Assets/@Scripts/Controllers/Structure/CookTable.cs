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
    onWorkSomeOne = true;
    StructureState = FStructureState.Work;
  }

  protected override void UpdateOnWork()
  {
    var want = Worker.GetJobPriority(workableJob);
    // 자는 사람이 있을 때 
    if (Worker != null)
    {
      StartCoroutine(ReduceSleepy(want));
    }


  }

  private IEnumerator ReduceSleepy(float want)
  {
    while (true)
    {
      yield return null;
      want -= 1;
      if (want <= 0)
      {
        StructureState = FStructureState.WorkEnd;
        break;
      }
    }
  }

  protected override void UpdateWorkEnd()
  {
    //Storage.AddRange(Worker.SupplyStorage);
    //Worker.SupplyStorage.Clear();
    //Worker.CurrentSupply = 0;
    //Worker.jobSystem.supplyTargets.Clear();
    if (Worker == null) return;
    Worker.ppSystem.target = null;
    Worker.SetOrAddJobPriority(workableJob, 0, true);
    Worker.Target = null;
    Worker.RestartCo();
    Worker.StartAnimation();
    Worker = null;

    StructureState = FStructureState.Idle;
    onWorkSomeOne = false;

  }
}
