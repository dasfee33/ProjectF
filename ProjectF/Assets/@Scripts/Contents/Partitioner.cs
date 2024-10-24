using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partitioner : InitBase
{
  public List<PartitionerLayer> layers = new List<PartitionerLayer>();

  public override bool Init()
  {
    if (base.Init() == false) return false;

    Managers.Partition.CreateMask("PlowPartition", layers);
    //Add TODO

    return true;
  }

  public Partitioner(int node_size, int layer_count, int scene_width, int scene_height)
  {
    //this.nodeSize = node_size;
    //int length1 = scene_width / node_size;
    //int length2 = scene_height / node_size;

  }

}
