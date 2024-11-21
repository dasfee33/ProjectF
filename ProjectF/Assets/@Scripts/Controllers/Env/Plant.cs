using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Env
{
  //public bool harvestReady = false;

  //private int default_growth = 0;
  //public override bool Init()
  //{
  //  if (base.Init() == false) return false;

  //  default_growth = UnityEngine.Random.Range(0, 6);
  //  SpriteRenderer.sprite = Managers.Resource.Load<Sprite>($"kick{default_growth}");

  //  StartCoroutine(Growth());

  //  return true;
  //}

  //private IEnumerator Growth()
  //{
  //  while(default_growth < 5)
  //  {
  //    yield return new WaitForSeconds(base.data.GrowthTime / 5);
  //    default_growth += 1;
  //    SpriteRenderer.sprite = Managers.Resource.Load<Sprite>($"kick{default_growth}");
  //  }
  //  harvestReady = true;
  //}
}
