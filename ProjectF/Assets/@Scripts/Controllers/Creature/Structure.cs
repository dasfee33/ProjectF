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
  private StructureData data;
  private FStructureState structureState = FStructureState.Idle;

  public List<BaseObject> Storage = new List<BaseObject>();

  public FStructureState StructureState
  {
    get { return structureState; }
    set
    {
      structureState = value;
      UpdateAnimation();
    }
  }
  private FStructureType structureType = FStructureType.None;
  public FStructureType StructureType
  {
    get { return structureType; }
    set
    {
      structureType = value;
      UpdateJob();
    }
  }

  #region Stats
  public float Hp { get; set; }
  public float maxHp { get; set; }
  public float workTime { get; set; }
  public float supplyItemid { get; set; }
  #endregion

  public override bool Init()
  {
    if (base.Init() == false)
      return false;

    ObjectType = FObjectType.Structure;

    return true;
  }

  public void SetInfo(int dataID)
  {
    dataTemplateID = dataID;
    data = Managers.Data.StructDic[dataID];

    Hp = data.maxHp;
    maxHp = data.maxHp;
    workTime = data.WorkTime;
    if (Enum.TryParse(data.type, out FStructureType result))
      StructureType = result;
    supplyItemid = data.supplyItemid;
  }

  private void UpdateJob()
  {
    switch (structureType)
    {
      case FStructureType.Toilet:
        workableJob = FPersonalJob.Excretion;
        break;
      case FStructureType.Bed:
        workableJob = FPersonalJob.Sleepy;
        break;
      case FStructureType.EatingTable:
        workableJob = FPersonalJob.Hungry;
        break;
      case FStructureType.Chest:
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
        ResetState();
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
    if (maxHp < 0) return;

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

  public void OnAnimIsEnd()
  {
    if(StructureState == FStructureState.WorkStart)
    {
      StructureState = FStructureState.Work;
      if (Worker != null)
      {
        if (StructureType == FStructureType.Toilet)
        {
          Worker.SpriteRenderer.DOFade(0, 1f).OnComplete(() => { onWorkSomeOne = true; });
        }
        else onWorkSomeOne = true;
      }

      StartCoroutine(Working());
    }
    else if(StructureState == FStructureState.WorkEnd)
    {
      if(StructureType == FStructureType.Toilet)
      {
        Worker.SpriteRenderer.DOFade(1, 1f);
        Worker.ppSystem.target = null;
        Worker.SetOrAddJobPriority(workableJob, 0, true);
        Worker.Target = null;
        onWorkSomeOne = false;
        Worker = null;
        StructureState = FStructureState.Idle;
      }
    }
  }

  private void ResetState()
  {
    if (StructureType == FStructureType.Toilet) return;

    StructureState = FStructureState.Idle;
    if (Worker != null)
    {
      if(StructureType == FStructureType.Chest)
      {
        Storage.AddRange(Worker.SupplyStorage);
        Worker.SupplyStorage.Clear();
        Worker.CurrentSupply = 0;
        Worker.jobSystem.supplyTargets.Clear();
        Worker.jobSystem.target = null;
        Worker.SetOrAddJobPriority(workableJob, 0, true);
        Worker.Target = null;
        onWorkSomeOne = false;
        Worker = null;
      }
      

    }
  }

  private IEnumerator Working()
  {
    yield return new WaitForSeconds(workTime);

    StructureState = FStructureState.WorkEnd;
  }
}
