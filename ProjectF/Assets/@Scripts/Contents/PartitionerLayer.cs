using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartitionerLayer
{
  public string name;
  public int layer;
  public System.Action<int, object> OnEvent;

  public PartitionerLayer(string name, int layer)
  {
    this.name = name;
    this.layer = layer;
  }
}
