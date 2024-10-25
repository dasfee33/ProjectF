using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ToolBase : InitBase
{
  public GameObject[] ToolList;
  private Grid grid;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    grid = this.GetComponent<Grid>();

    return true;
  }

  private void resetTool()
  {
    foreach (GameObject tool in ToolList)
      tool.SetActive(false);
  }

  private void Update()
  {
    //TEMP
    if(Input.GetKeyDown(KeyCode.F))
    {
      resetTool();
      GameObject go = ToolList[(int)FTool.Plow];
      go.SetActive(true);
      go.GetComponent<PlowTool>().grid = this.grid;
    }
  }
}
