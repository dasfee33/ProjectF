using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

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

  public void Move(PointerEventData evt)
  {
    var camera = Camera.main;
    Vector3 sceenDelta = new Vector3(evt.delta.x, evt.delta.y);
    Vector3 worldDelta = camera.ScreenToWorldPoint(sceenDelta + camera.WorldToScreenPoint(Owner.transform.position)) -
      Owner.transform.position;

    Owner.transform.position += worldDelta;
  }

  public void Confirm(PointerEventData evt)
  {
    var cellPos = Managers.Map.World2Cell(Owner.transform.position);
    Owner.transform.position = Managers.Map.Cell2World(cellPos);

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
