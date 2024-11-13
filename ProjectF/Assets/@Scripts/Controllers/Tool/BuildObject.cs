using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BuildObject : BaseObject
{
  private List<int> buildItemList = new List<int>();
  private List<float> buildItemMass = new List<float>();

  /// <summary>
  /// key : dataID
  /// value : mass
  /// </summary>
  public Dictionary<int, float> buildNeedList = new Dictionary<int, float>();

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
    grid = Managers.Map.CellGrid;
    startCellPos = Managers.Map.CellGrid.WorldToCell(this.transform.position);

    Managers.FInput.startTouch -= StartTouch;
    Managers.FInput.startTouch += StartTouch;

    Managers.FInput.onDragging -= IsDragging;
    Managers.FInput.onDragging += IsDragging;

    Managers.FInput.endTouch -= EndTouch;
    Managers.FInput.endTouch += EndTouch;
    return true;
  }

  public void SetInfo(int id, List<int> a, List<float> b)
  {
    buildItemList = a;
    buildItemMass = b;

    if (buildItemList.Count != buildItemMass.Count) return;

    for(int i = 0; i < buildItemList.Count; i++)
    {
      buildNeedList.Add(buildItemList[i], buildItemMass[i]);
    }

    workableJob = FJob.Supply;
  }

  public override void OnDamaged(BaseObject attacker)
  {
    var attackOwner = attacker as Creature;

    foreach(var item in buildNeedList)
    {
      if (attackOwner.SearchHaveList(item.Key))
      {
        float result = attackOwner.SupplyFromHaveList(item.Key, item.Value);
        buildNeedList[item.Key] -= result;
      }
    }

    CheckBuildReady();
  }

  private bool CheckBuildReady()
  {
    foreach(var value in buildNeedList.Values)
    {
      if (value != 0) return false;
    }

    workableJob = FJob.Make;
    return true;
  }


  #region Input
  private void StartTouch(Vector2 pos, float time)
  {
    worldPos = Camera.main.ScreenToWorldPoint(pos);
    worldPos -= Managers.Map.LerpObjectPos;
    worldPos.z = 0f;
    if (Managers.Map.GetObject(worldPos) == this)
    {
      startCellPos = Managers.Map.World2Cell(worldPos);
      isMe = true;
    }
  }

  private void IsDragging(Vector2 pos)
  {
    if (!isMe) return;
    worldPos = Camera.main.ScreenToWorldPoint(pos);
    worldPos -= Managers.Map.LerpObjectPos;
    worldPos.z = 0f;

    //Lerp Position
    cellPos = Managers.Map.World2Cell(worldPos);
    cellWorldPos = Managers.Map.Cell2World(cellPos) + Managers.Map.LerpObjectPos;
    this.transform.position = cellWorldPos;
  }

  private void EndTouch(Vector2 pos, float time)
  {
    if (!isMe) return;
    this.transform.position = cellWorldPos;
    var toolBase = Managers.Map.Map.GetComponent<ToolBase>();
    Managers.Map.ClearObject(startCellPos);
    Managers.Map.AddObject(this, cellPos);
    isMe = !isMe;
  }
  #endregion
}
