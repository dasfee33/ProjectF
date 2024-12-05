using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Trunk : Env
{
  private float regenTime;
  private int dataId;

  private Data.EnvData _data;

  public override bool Init()
  {
    if (base.Init() == false) return false;
    ObjectType = FObjectType.Env;
    

    return true;
  }

  private void Start()
  {
    wait = new WaitForSeconds(regenTime);
    StartCoroutine(Regen());
  }

  //public void SetInfo(Data.EnvData data)
  //{
  //  _data = data;
  //  dataId = data.DataId;
  //  regenTime = data.RegenTime;
  //  wait = new WaitForSeconds(regenTime);

    
  //}

  private WaitForSeconds wait;
  private IEnumerator Regen()
  {
    yield return wait;
    Managers.Object.Spawn<Env>(this.transform.position, dataId, _data.Name);
    Object.Destroy(this.gameObject);
  }
}
