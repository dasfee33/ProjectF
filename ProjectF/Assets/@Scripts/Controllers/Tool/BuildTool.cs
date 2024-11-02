using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class BuildTool : MonoBehaviour
{
  public Grid grid;
  public Tilemap tilemap;

  private GameObject BuildObject;
  private SpriteRenderer BuildObjectSprite;

  private Vector3Int cellPos;
  private Vector3 cellWorldPos;

  private bool setEnd;

  public string toolTag => SetTag("Build");

  private void Awake()
  {
    if (BuildObject == null) BuildObject = Managers.Object.Spawn<Env>(Vector3.zero, ENV_CHEST_NORMAL, "CabinChest").gameObject;
    BuildObjectSprite = BuildObject.GetComponent<SpriteRenderer>();

    SetTag(tag);
  }

  private string SetTag(string tag)
  {
    this.tag = tag;

    return this.tag;
  }

  private void Update()
  {
    if (setEnd) return;
    Vector3 mousePos = Input.mousePosition;
    Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
    worldPos.z = 0;

    //클릭 했을 때 
    if(Input.GetMouseButtonUp(0))
    {
      BuildObject.transform.position = cellWorldPos;
      if (BuildObjectSprite.color.a <= 0.5f)
      {
        Color color = new Vector4(BuildObjectSprite.color.r, BuildObjectSprite.color.g, BuildObjectSprite.color.b, 1);
        BuildObjectSprite.color = color;
      }

      setEnd = true;
    }
    else
    {
      cellPos = grid.WorldToCell(worldPos);
      cellWorldPos = grid.CellToWorld(cellPos);
      BuildObject.transform.position = cellWorldPos;

      if (BuildObjectSprite.color.a > 0.5f)
      {
        Color color = new Vector4(BuildObjectSprite.color.r, BuildObjectSprite.color.g, BuildObjectSprite.color.b, 0.5f);
        BuildObjectSprite.color = color;
      }
    }
  }

}
