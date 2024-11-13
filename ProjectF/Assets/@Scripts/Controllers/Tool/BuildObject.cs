using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BuildObject : BaseObject
{
  private List<int> buildItemList = new List<int>();
  private List<int> buildItemCount = new List<int>();

  private Dictionary<int, int> buildList = new Dictionary<int, int>();

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

  public void SetInfo(int id, List<int> a, List<int> b)
  {
    buildItemList = a;
    buildItemCount = b;

    if (buildItemList.Count != buildItemCount.Count) return;

    for(int i = 0; i < buildItemList.Count; i++)
    {
      buildList.Add(buildItemList[i], buildItemCount[i]);
    }
  }

  private void StartTouch(Vector2 pos, float time)
  {
    worldPos = Camera.main.ScreenToWorldPoint(pos);
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
    worldPos.z = 0f;

    //Lerp Position
    cellPos = Managers.Map.World2Cell(worldPos);
    cellWorldPos = Managers.Map.Cell2World(cellPos) + new Vector3(0.16f, 0.16f);
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
}
