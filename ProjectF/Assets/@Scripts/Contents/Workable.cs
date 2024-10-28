using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Workable : InitBase
{
  public BaseObject Owner { get; protected set; }
  public FJob Job { get; protected set; }

  public override bool Init()
  {
    if (base.Init() == false) return false;



    return true;
  }

}
