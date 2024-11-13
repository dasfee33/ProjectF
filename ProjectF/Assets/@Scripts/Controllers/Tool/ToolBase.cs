using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class ToolBase : InitBase
{
  public GameObject[] ToolLayerList;
  public GameObject[] ToolList;
  private Grid grid;

  public bool isBuild = false;
  public bool isPlow = false;

  public Data.StructureData objData;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    grid = this.GetComponent<Grid>();

    return true;
  }

  private void resetTool()
  {
    foreach (GameObject tool in ToolLayerList)
      tool.SetActive(false);
  }

  private void Update()
  {
    //TEMP
    if(Input.GetKeyDown(KeyCode.F))
    {
      resetTool();
      GameObject go = ToolLayerList[(int)FTool.Plow];
      go.SetActive(true);
      PlowTool plow = go.GetComponent<PlowTool>();
      plow.grid = this.grid;
      plow.tilemap = ToolList[(int)FTool.Plow].GetComponent<Tilemap>();

    }

    if (isBuild)
    {
      resetTool();
      GameObject go = ToolLayerList[(int)FTool.Build];
      go.SetActive(true);
      BuildTool build = go.GetComponent<BuildTool>();
      build.parent = this;
      //build.grid = this.grid;
      //build.tilemap = ToolList[(int)FTool.Build].GetComponent<Tilemap>();
      build.SetInfo();
      isBuild = false;
    }
  }
}
