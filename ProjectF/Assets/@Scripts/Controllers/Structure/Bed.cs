using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Bed : Structure
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
    StructureSubType = FStructureSubType.Bed;
    spriteRenderer.sortingOrder = 19;

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

    LerpCellPosCompleted = true;
    if (Worker._coai != null) { Worker.CancelCo(); Worker.StopAnimation(); }
    Worker.SpriteRenderer.sprite = Managers.Resource.Load<Sprite>("warrior-sleep");
    Worker.transform.position = this.transform.position;
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
      //틱 뒤에 다시 들어오면 일 끝남처리
      StructureState = FStructureState.WorkEnd;
    }
  }

  protected override void UpdateWorkEnd()
  {
    //Storage.AddRange(Worker.SupplyStorage);
    //Worker.SupplyStorage.Clear();
    //Worker.CurrentSupply = 0;
    //Worker.jobSystem.supplyTargets.Clear();
    Worker.SetOrAddJobPriority(workableJob, 0, true);
    Managers.Event.MoodStable(Worker, workableJob);
    Worker.RestartCo();
    Worker.StartAnimation();
    Worker.ResetJob();

    StructureState = FStructureState.Idle;
    onWorkSomeOne = false;

  }


}
