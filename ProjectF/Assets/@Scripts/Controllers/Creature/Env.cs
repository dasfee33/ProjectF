using System;
using UnityEngine;
using static Define;
using Data;

public class Env : BaseObject
{
  private Vector3 dropPos;
  private EnvData data;
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
    // hp 가 없는 일반 환경사물 (ex 상자)
    if (maxHp < 0) return;

    //TEMP
    DroppedItem();


    Hp = Mathf.Clamp(Hp - finalDamage, 0, maxHp);
    if (Hp <= 0)
    {
      OnDead(attacker);
    }
  }

  private void DroppedItem()
  {
    int dropItemId = data.DropItemid;
    RewardData rewardData = GetRandomReward();
    ItemHolder dropItem;
    if (rewardData != null)
    {
      //TEMP
      if (droppedItem == null)
      {
        Vector3 rand = new Vector3(transform.position.x + UnityEngine.Random.Range(-2, -5) * 0.1f, transform.position.y);
        Vector3 rand2 = new Vector3(transform.position.x + UnityEngine.Random.Range(2, 5) * 0.1f, transform.position.y);
        dropPos = UnityEngine.Random.value < 0.5 ? rand : rand2;        
      }
      dropItem = Managers.Object.Spawn<ItemHolder>(transform.position, dropItemId, addToCell: false);
      dropItem.Owner = this;
      dropItem.SetInfo(0, rewardData.itemTemplateId, dropPos);
    }
  }

  public override void OnDead(BaseObject attacker)
  {
    base.OnDead(attacker);

    EnvState = FEnvState.Dead;
  }

  private RewardData GetRandomReward()
  {
    if (data == null)
      return null;

    if (Managers.Data.DropDic.TryGetValue(data.DataId, out DropTableData dropTableData) == false)
      return null;

    if (dropTableData.Rewards.Count <= 0)
      return null;

    int sum = 0;
    int randomValue = UnityEngine.Random.Range(0, 100);

    foreach (RewardData item in dropTableData.Rewards)
    {
      sum += item.Probability;

      if (randomValue <= sum)
      {
        return item;
      }
    }

    return null;
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
