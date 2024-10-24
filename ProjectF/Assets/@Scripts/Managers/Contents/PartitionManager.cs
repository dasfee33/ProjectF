using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartitionManager
{

  public PartitionerLayer CreateMask(string name, List<PartitionerLayer> layers)
  {
    if(layers == null)
    {
      Debug.LogError($"not exist Partition");
      return null;
    }

    foreach (PartitionerLayer layer in layers)
    {
      if (layer.name == name)
        return layer;
    }

    PartitionerLayer mask = new PartitionerLayer(name, layers.Count);
    layers.Add(mask);

    return mask;
  }

}
