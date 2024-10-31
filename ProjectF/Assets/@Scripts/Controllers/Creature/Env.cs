using System;
using UnityEngine;
using static Define;

public class Env : BaseObject
{
  private Data.EnvData data;
  private bool deadFlag = false;

  private FEnvState envState = FEnvState.Idle;
  public FEnvState EnvState
  {
    get { return envState; }
    set
    {
      envState = value;
      UpdateAnimation();
    }
  }
  private FEnvType envType = FEnvType.None;
  public FEnvType EnvType
  {
    get { return envType;}
    set
    {
      envType = value;
      UpdateJob();
    }
  }

  #region Stats
  public float Hp { get; set; }
  public float maxHp { get; set; }
  public float regenTIme { get; set; }

  #endregion

  public override bool Init()
  {
    if (base.Init() == false)
      return false;

    ObjectType = FObjectType.Env;

    return true;
  }

  public void SetInfo(int dataID)
  {
    dataTemplateID = dataID;
    data = Managers.Data.EnvDic[dataID];

    Hp = data.maxHp;
    maxHp = data.maxHp;
    regenTIme = data.RegenTime;
    if (Enum.TryParse(data.type, out FEnvType result))
      EnvType = result;
    
  }

  private void UpdateJob()
  {
    switch(envType)
    {
      case FEnvType.Tree:
        workableJob = FJob.Logging;
        break;
      //TODO;
    }
  }

  protected override void UpdateAnimation()
  {
    switch(EnvState)
    {
      case FEnvState.Idle:
        PlayAnimation(data.Idle);
        break;
      case FEnvState.Hurt:
        PlayAnimation(data.Hurt);
        break;
      case FEnvState.Dead:
        PlayAnimation(data.Dead);
        break;

    }
  }

  public override void OnDamaged(BaseObject attacker)
  {
    if (EnvState == FEnvState.Dead)
    {
      return;
    }

    base.OnDamaged(attacker);
    //TODO
    float finalDamage = attacker.GetComponent<Creature>().Skills[0].DamageMultiflier;
    EnvState = FEnvState.Hurt;

    Hp = Mathf.Clamp(Hp - finalDamage, 0, maxHp);
    if (Hp <= 0)
    {
      OnDead(attacker);
    }
  }

  public override void OnDead(BaseObject attacker)
  {
    base.OnDead(attacker);

    EnvState = FEnvState.Dead;
  }

  public void OnDespawn()
  {
    Managers.Object.Despawn(this);
  }

  public void OnAnimIsEnd()
  {
    if (EnvState == FEnvState.Dead) return;
    EnvState = FEnvState.Idle;
  }
}
