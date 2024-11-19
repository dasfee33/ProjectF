using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static COLOR;

public class UI_WorldUITest : UI_Base
{
  public BuildObject Owner;

  public enum Buttons
  {
    CancelButton,
    Move,
    ConfirmButton,
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindButtons(typeof(Buttons));

    GetButton((int)Buttons.CancelButton).gameObject.BindEvent(Cancel, FUIEvent.Click);
    GetButton((int)Buttons.Move).gameObject.BindEvent(Move, FUIEvent.Drag);
    GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(Confirm, FUIEvent.Click);

    return true;
  }

  public void SetInfo(BuildObject owner)
  {
    Owner = owner;
  }

  public void Cancel(PointerEventData evt)
  {

  }

  Vector3 diff;
  public void Move(PointerEventData evt)
  {
    var camera = Camera.main;
    if (diff == Vector3.zero)
      diff = Owner.transform.position - this.transform.position;

    var worldPos = camera.ScreenToWorldPoint(evt.position);
    worldPos.z = 0f;

    //Lerp Position
    var cellPos = Managers.Map.World2Cell(worldPos);
    var cellWorldPos = Managers.Map.Cell2World(cellPos);
    Owner.transform.position = cellWorldPos + Managers.Map.LerpObjectPos + diff;

    var ownerCellPos = Managers.Map.World2Cell(cellWorldPos + diff);

    var cellX = Owner.data.extraCellX;
    var cellY = Owner.data.extraCellY;
    for (int dx = -cellX; dx <= cellX; dx++)
    {
      for (int dy = -cellY; dy <= cellY; dy++)
      {
        Vector3Int checkCellPos = new Vector3Int(ownerCellPos.x + dx, ownerCellPos.y + dy);
        BaseObject prev = Managers.Map.GetObject(checkCellPos);

        if (prev != null && prev != Owner)
        {
          Owner.SetColor(COLOR.SMOKERED);
          return;
        }
        else { Owner.SetColor(COLOR.SMOKEWHITE); return; }
      }
    }
  }

  public void Confirm(PointerEventData evt)
  {
    //var cellPos = Managers.Map.World2Cell(Owner.transform.position);
    //Owner.transform.position = Managers.Map.Cell2World(cellPos);

    Owner.setFlag = true;

    Managers.Game.OnJobAbleChanged(Owner.workableJob, true);

    this.gameObject.SetActive(false);
  }

  //private void StartTouch(Vector2 pos, float time)
  //{
  //  worldPos = Camera.main.ScreenToWorldPoint(pos);
  //  worldPos -= Managers.Map.LerpObjectPos;
  //  worldPos.z = 0f;
  //  if (Managers.Map.GetObject(worldPos) == this)
  //  {
  //    startCellPos = Managers.Map.World2Cell(worldPos);
  //    isMe = true;
  //  }
  //}

  //private void IsDragging(Vector2 pos)
  //{
  //  worldPos = Camera.main.ScreenToWorldPoint(pos);
  //  worldPos.z = 0f;

  //  //Lerp Position
  //  cellPos = Managers.Map.World2Cell(worldPos);
  //  cellWorldPos = Managers.Map.Cell2World(cellPos);
  //  this.transform.position = cellWorldPos;
  //}

  //private void EndTouch(Vector2 pos, float time)
  //{
  //  if (!isMe) return;
  //  this.transform.position = cellWorldPos;
  //  var toolBase = Managers.Map.Map.GetComponent<ToolBase>();
  //  Managers.Map.ClearObject(startCellPos);
  //  Managers.Map.AddObject(this, cellPos);
  //  isMe = !isMe;
  //}
}
