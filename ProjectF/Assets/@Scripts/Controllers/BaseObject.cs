using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Data;

public class BaseObject : InitBase
{
  public int ExtraCellsX { get; set; } = 0;
  public int ExtraCellsY { get; set; } = 0;
  public bool Option { get; set; } = false;
  public ItemHolder droppedItem;

  public FObjectType ObjectType { get; protected set; } = FObjectType.None;
  public Rigidbody2D RigidBody { get; private set; }
  public CapsuleCollider2D Collider { get; private set; }
  public SpriteRenderer SpriteRenderer { get; private set; }
  public Animator Animator { get; private set; }

  public float ColliderRadius { get { return Collider != null ? Collider.size.y : 0.0f; } }
  public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

  protected Vector3 previousPos = Vector3.zero;
  protected Vector3 currentPos = Vector3.zero;

  public int dataTemplateID { get; set; }

  public Enum job;
  public Enum workableJob;

  public Creature Worker;

  public bool onWork = false;
  public bool onWorkSomeOne = false;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    RigidBody = this.GetComponent<Rigidbody2D>();
    Collider = this.GetComponent<CapsuleCollider2D>();
    SpriteRenderer = this.GetComponent<SpriteRenderer>();
    Animator = this.GetComponent<Animator>();
    if(Animator == null) Animator = this.GetComponentInChildren<Animator>();

    //TEMP
    SpriteRenderer.sortingOrder = 20;

    return true;
  }

  #region Anim
  private bool _lookLeft = true;
  public bool LookLeft
  {
    get { return _lookLeft; }
    set
    {
      _lookLeft = value;
      Flip(!value);
    }
  }

  protected virtual void UpdateAnimation() { }

  public void PlayAnimation(string anim)
  {
    if (Animator == null) return;

    Animator.Play(anim, -1);
  }

  public void StopAnimation() => Animator.enabled = false;

  public void StartAnimation() => Animator.enabled = true;

  public void Flip(bool flag)
  {
    if (SpriteRenderer == null) return;

    currentPos = transform.position;
    if (currentPos.x < previousPos.x) SpriteRenderer.flipX = true;
    else if(currentPos.x > previousPos.x) SpriteRenderer.flipX = false;

    previousPos = currentPos;

  }

  public void TranslateEx(Vector3 dir)
  {
    transform.Translate(dir);

    if (dir.x < 0) LookLeft = true;
    else if (dir.x > 0) LookLeft = false;
  }

  #endregion

  #region Misc
  public bool IsValid(BaseObject bo)
  {
    return bo.IsValid();
  }

  #endregion

  #region Map
  public bool LerpCellPosCompleted { get; protected set; }

  Vector3Int _cellPos;
  public Vector3Int CellPos
  {
    get { return _cellPos; }
    protected set
    { 
      _cellPos = value;
      LerpCellPosCompleted = false;
    }
  }

  public void SetCellPos(Vector3Int cellPos, bool forceMove = false)
  {
    CellPos = cellPos;
    LerpCellPosCompleted = false;

    if (forceMove)
    {
      transform.position = Managers.Map.Cell2World(CellPos);
      LerpCellPosCompleted = true;
    }
  }

  public void LerpToCellPos(float moveSpeed)
  {
    if (LerpCellPosCompleted)
      return;

    Debug.Log($"현재 {name} 의 일은 {job} 입니다.");
    Debug.Log($"현재 {name} 의 용무는 {workableJob} 입니다.");


    Vector3 destPos = Managers.Map.Cell2World(CellPos);
    Vector3 dir = destPos - transform.position;

    if (dir.x < 0)
      LookLeft = true;
    else
      LookLeft = false;

    if (dir.magnitude < 0.01f)
    {
      transform.position = destPos;
      LerpCellPosCompleted = true;
      return;
    }

    float moveDist = Mathf.Min(dir.magnitude, moveSpeed * Time.deltaTime);
    transform.position += dir.normalized * moveDist;
  }
  #endregion

  #region Battle
  public virtual void OnDamaged(BaseObject attacker)
  {

  }

  public virtual void OnDead(BaseObject attacker)
  {

  }

  public virtual void OnAnimEventHandler()
  {

  }

  #endregion
}
