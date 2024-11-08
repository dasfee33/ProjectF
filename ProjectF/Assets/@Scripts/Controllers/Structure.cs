using System;
using UnityEngine;
using static Define;
using Data;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Structure : BaseObject
{
  private Vector3 dropPos;
  public SpriteRenderer spriteRenderer;
  protected StructureData data;

  public List<BaseObject> Storage = new List<BaseObject>();

  protected FStructureState structureState = FStructureState.Idle;
  public virtual FStructureState StructureState
  {
    get { return structureState; }
    set
    {
      structureState = value;
      UpdateAnimation();
    }
  }

  protected FStructureType structureType = FStructureType.None;
  public FStructureType StructureType
  {
    get { return structureType; }
    set
    {
      structureType = value;
    }
  }

  protected FStructureSubType structureSubType = FStructureSubType.None;
  public FStructureSubType StructureSubType
  {
    get { return structureSubType; }
    set
    {
      structureSubType = value;
      UpdateJob();
    }
  }

  #region Stats
  private float _hp { get; set; }
  private float _maxHp { get; set; }
  private float _workTime { get; set; }
  private float _supplyItemid { get; set; }

  public float Hp { get { return _hp; }set { _hp = value; } }
  public float MaxHp { get { return _maxHp; }set { _maxHp = value; } }
  public float WorkTime { get { return _workTime; }set { _workTime = value; } }
  public float SupplyItemId { get { return _supplyItemid; }set { _supplyItemid = value; } }
  #endregion

  public override bool Init()
  {
    if (base.Init() == false)
      return false;

    ObjectType = FObjectType.Structure;
    spriteRenderer = this.GetComponent<SpriteRenderer>();

    return true;
  }

  public void SetInfo(int dataID)
  {
    dataTemplateID = dataID;
    data = Managers.Data.StructDic[dataID];

    _hp = data.maxHp;
    _maxHp = data.maxHp;
    _workTime = data.WorkTime;

    gameObject.name = $"{data.DataId}_{data.Name}";

    structureState = FStructureState.Idle;
  }

  public float UpdateAITick { get; protected set; } = 0.0f;
  protected IEnumerator CoUpdateAI()
  {
    while (true)
    {
      switch (StructureState)
      {
        case FStructureState.Idle:
          UpdateIdle();
          break;
        case FStructureState.WorkStart:
          UpdateWorkStart();
          break;
        case FStructureState.Work:
          UpdateOnWork();
          break;
        case FStructureState.WorkEnd:
          UpdateWorkEnd();
          break;
      }

      if (UpdateAITick > 0.0f) yield return new WaitForSeconds(UpdateAITick);
      else yield return null;
    }
  }

  protected virtual void UpdateIdle() { }
  protected virtual void UpdateWorkStart() { }
  protected virtual void UpdateOnWork() { }
  protected virtual void UpdateWorkEnd() { }


  private void UpdateJob()
  {
    switch (structureSubType)
    {
      case FStructureSubType.Toilet:
        workableJob = FPersonalJob.Excretion;
        break;
      case FStructureSubType.Bed:
        workableJob = FPersonalJob.Sleepy;
        break;
      case FStructureSubType.EatingTable:
        workableJob = FPersonalJob.Hungry;
        break;
      case FStructureSubType.Storage:
        workableJob = FJob.Store;
        break;
        //TODO;
    }
  }

  protected override void UpdateAnimation()
  {
    switch (StructureState)
    {
      case FStructureState.Idle:
        PlayAnimation(data.Idle);
        break;
      case FStructureState.WorkStart:
        PlayAnimation(data.WorkStart);
        break;
      case FStructureState.Work:
        PlayAnimation(data.Work);
        break;
      case FStructureState.WorkEnd:
        PlayAnimation(data.WorkEnd);
        break;

    }
  }

  public override void OnDamaged(BaseObject attacker)
  {
    if (StructureState == FStructureState.Work || StructureState == FStructureState.WorkEnd)
    {
      return;
    }

    base.OnDamaged(attacker);
    //TODO
    //float finalDamage = attacker.GetComponent<Creature>().Skills[0].DamageMultiflier;
    StructureState = FStructureState.WorkStart;
    // hp 가 없는 일반 환경사물 (ex 상자)g
    if (_maxHp < 0) return;

    //Hp = Mathf.Clamp(Hp - finalDamage, 0, maxHp);
    //if (Hp <= 0)
    //{
    //  OnDead(attacker);
    //}
  }

  

  //public override void OnDead(BaseObject attacker)
  //{
  //  base.OnDead(attacker);

  //  EnvState = FEnvState.Dead;
  //}


  public void OnDespawn()
  {
    Managers.Object.Despawn(this);
  }

  


}
