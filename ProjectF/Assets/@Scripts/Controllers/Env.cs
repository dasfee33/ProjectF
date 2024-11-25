using System;
using UnityEngine;
using static Define;
using Data;
using System.Collections;

public class Env : BaseObject
{
  public bool isFarm = false;
  public bool harvestIsReady = false;
  private Vector3 dropPos;
  protected EnvData data;
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

  public void SetInfo(int dataID, bool isFarm)
  {
    dataTemplateID = dataID;
    data = Managers.Data.EnvDic[dataID];

    Hp = data.maxHp;
    maxHp = data.maxHp;
    regenTIme = data.RegenTime;
    this.isFarm = isFarm;
    if (Enum.TryParse(data.type, out FEnvType result))
      EnvType = result;
    
  }

  private void UpdateJob()
  {
    switch(envType)
    {
      case FEnvType.Tree:
        harvestIsReady = true;
        workableJob = FJob.Logging;
        break;
      case FEnvType.Rock:
        harvestIsReady = true;
        workableJob = FJob.Dig;
        break;
      case FEnvType.Plant:
        workableJob = FJob.Plow;
        int default_growth = 0;
        if(!isFarm) default_growth = UnityEngine.Random.Range(0, 6);
        SpriteRenderer.sprite = Managers.Resource.Load<Sprite>($"kick{default_growth}");
        StartCoroutine(Growth(default_growth));
        break;

      //TODO;
    }
  }

  private IEnumerator Growth(int growth)
  {
    var g = growth;
    while (g < 5)
    {
      yield return new WaitForSeconds(data.GrowthTime / 5);
      g += 1;
      SpriteRenderer.sprite = Managers.Resource.Load<Sprite>($"kick{g}");
    }
    harvestIsReady = true;
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
    //int dropItemId = data.DropItemid;
    RewardData rewardData = GetRandomReward();
    //ItemHolder dropItem;
    if (rewardData != null)
    {
      //TEMP
      if (droppedItem == null)
      {
        Vector3 rand = new Vector3(transform.position.x + UnityEngine.Random.Range(-2f, -5f) * 0.1f, transform.position.y);
        Vector3 rand2 = new Vector3(transform.position.x + UnityEngine.Random.Range(2f, 5f) * 0.1f, transform.position.y);
        dropPos = UnityEngine.Random.value < 0.5 ? rand : rand2;        
      }
      Managers.Object.Spawn<ItemHolder>(transform.position, rewardData.itemTemplateId, addToCell: false, dropPos: dropPos, Owner: this);
    }
  }

  public override void OnDead(BaseObject attacker)
  {
    base.OnDead(attacker);

    switch(EnvType)
    {
      case FEnvType.Tree:
        var trunk = Managers.Object.Spawn<Trunk>(this.transform.position, dataTemplateID, prefabName: data.Name + "_trunk", addToCell: false);
        trunk.SetInfo(data);
        break;

    }
    

    EnvState = FEnvState.Dead;
    if (Animator == null) OnDespawn();
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
