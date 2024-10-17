using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{
  public FObjectType ObjectType { get; protected set; } = FObjectType.None;
  public Rigidbody2D RigidBody { get; private set; }
  public SpriteRenderer SpriteRenderer { get; private set; }
  public Animator Animator { get; private set; }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    RigidBody = this.GetComponent<Rigidbody2D>();
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
    if (RigidBody == null) return;
    if (SpriteRenderer == null) return;

    _ = RigidBody.velocity.x >= 0 ? SpriteRenderer.flipX = false : SpriteRenderer.flipX = true;
  }

  #endregion
}
