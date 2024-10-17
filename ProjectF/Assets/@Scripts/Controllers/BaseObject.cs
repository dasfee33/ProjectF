using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{
  public FObjectType ObjectType { get; protected set; } = FObjectType.None;
  public Rigidbody2D RigidBody { get; private set; }
  public CapsuleCollider2D Collider { get; private set; }
  public SpriteRenderer SpriteRenderer { get; private set; }
  public Animator Animator { get; private set; }

  public float ColliderRadius { get { return Collider != null ? Collider.size.y : 0.0f; } }
  public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

  public Vector3 previousPos = Vector3.zero;
  public Vector3 currentPos = Vector3.zero;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    RigidBody = this.GetComponent<Rigidbody2D>();
    Collider = this.GetComponent<CapsuleCollider2D>();
    SpriteRenderer = this.GetComponent<SpriteRenderer>();
    Animator = this.GetComponent<Animator>();

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

  protected virtual void UpdateAnimation(FCreatureType type) { }

  public void PlayAnimation(string anim)
  {
    if (Animator == null) return;

    Animator.Play(anim);
  }

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
}
