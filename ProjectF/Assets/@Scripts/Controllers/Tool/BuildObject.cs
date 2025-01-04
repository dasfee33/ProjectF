using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BuildObject : Structure
{
  public UI_WorldUITest selectUI;
  public UI_BuildSlider sliderUI;
  public bool setFlag;

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
            UpdateAITick = data.BuildTime;
            break;
          case FStructureState.WorkEnd:
            UpdateAITick = 0f;
            break;
        }
      }
    }
  }

  private List<int> buildItemList = new List<int>();
  private List<float> buildItemMass = new List<float>();

  /// <summary>
  /// key : dataID
  /// value : mass
  /// </summary>
  public Dictionary<int, float> buildNeedList = new Dictionary<int, float>();
  public Dictionary<int, float> curNeedList;

  private Grid grid;
  private Vector3 worldPos;
  private Vector3Int cellPos;
  private Vector3 cellWorldPos;

  private Vector3Int startCellPos;

  private bool isMe = false;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    ObjectType = FObjectType.BuildObject;
    StructureType = FStructureType.BuildObject;
    StructureSubType = FStructureSubType.BuildObject;

    grid = Managers.Map.CellGrid;
    startCellPos = Managers.Map.CellGrid.WorldToCell(this.transform.position);

    selectUI.SetInfo(this);
    StartCoroutine(CoUpdateAI());
    return true;
  }

  public void SetInfo(int id, List<int> a, List<float> b)
  {
    dataTemplateID = id;
    data = Managers.Data.StructDic[id];
    buildItemList = a;
    buildItemMass = b;
    SpriteRenderer.sprite = Managers.Resource.Load<Sprite>(data.Sprite);
    SpriteRenderer.sortingOrder = 25;
    selectUI.GetComponent<Canvas>().sortingOrder = 25;

    if (buildItemList.Count != buildItemMass.Count) return;

    for(int i = 0; i < buildItemList.Count; i++)
    {
      buildNeedList.Add(buildItemList[i], buildItemMass[i]);
    }

    curNeedList = new Dictionary<int, float>(buildNeedList);

    workableJob = FJob.Supply;
    
  }

  public void SetColor(Vector4 color)
  {
    SpriteRenderer.color = color;
  }


  public override void OnDamaged(BaseObject attacker)
  {
    if(workableJob is FJob.Make)
    {
      base.OnDamaged(attacker);
      return;
    }

    var attackOwner = attacker as Creature;

    var saveList = new Dictionary<int, float>();

    foreach(var item in curNeedList)
    {
      if (attackOwner.SearchHaveList(item.Key))
      {
        float result = attackOwner.SupplyFromHaveList(item.Key, item.Value);
        saveList.Add(item.Key, result);
        //attackOwner.ResetJob();
      }
    }

    if(saveList.Count > 0)
    {
      foreach (var save in saveList)
        curNeedList[save.Key] -= save.Value;
    }

    CheckBuildReady();
    attackOwner.ResetJob();
  }

  private void CheckBuildReady()
  {
    foreach(var value in curNeedList.Values)
    {
      if (value != 0) return;
    }
    workableJob = FJob.Make;
  }

  protected override void UpdateIdle()
  {
    onWorkSomeOne = false;
  }

  protected override void UpdateWorkStart()
  {
    if (workableJob is not FJob.Make) return;

    onWorkSomeOne = true;
    StructureState = FStructureState.Work;
    sliderUI.gameObject.SetActive(true);
    sliderUI.SetInfo(data.BuildTime);
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
    if(sliderUI.coBuild is not null) StopCoroutine(sliderUI.coBuild);
    sliderUI.gameObject.SetActive(false);
    Worker.ResetJob();
    
    Worker = null;
    Managers.Map.ClearObject(GetCellPos());

    Managers.Object.Spawn<Structure>(this.transform.position/* - Managers.Map.LerpObjectPos*/, dataTemplateID, data.Name);
    Managers.Object.Despawn(this);
  }
}
