using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartitionManager : InitBase
{
  public PartitionerLayer plowChangeLayer;

  private Partitioner partitioner;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    CreateMask();

    return true;
  }

  public void CreateMask()
  {
    //this.plowChangeLayer = partio
  }

}
