using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class MapManager
{
  public GameObject Map { get; private set; }
  public string MapName { get; private set; }
  public Grid CellGrid { get; private set; }

  Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();

  private int MinX;
  private int MaxX;
  private int MinY;
  private int MaxY;

  public Vector3Int World2Cell(Vector3 worldPos) { return CellGrid.WorldToCell(worldPos); }
  public Vector3 Cell2World(Vector3Int cellPos) { return CellGrid.CellToWorld(cellPos); }

  FCellCollisionTypes[,] _collision;

  public void LoadMap(string mapName)
  {
    DestroyMap();

    GameObject map = Managers.Resource.Instantiate(mapName);
    map.transform.position = Vector3.zero;
    map.name = $"@Map_{mapName}";

    Map = map;
    MapName = mapName;
    CellGrid = map.GetComponent<Grid>();

    ParseCollisionData(map, mapName);
    
    CameraController cam = Camera.main.GetComponent<CameraController>();
    cam.confinerCam.m_BoundingShape2D = MakeMapCollisionBorder(map);
  }

  public void DestroyMap()
  {
    ClearObjects();

    if (Map != null)
      Managers.Resource.Destroy(Map);
  }

  private PolygonCollider2D MakeMapCollisionBorder(GameObject map)
  {
    Tilemap tilemap = map.transform.GetChild(0).GetComponent<Tilemap>();
    PolygonCollider2D collider;
    if (tilemap == null) return null;

    Vector3Int size = tilemap.size;
    Vector3 cellSize = tilemap.cellSize;

    float width = size.x * cellSize.x;
    float height = size.y * cellSize.y;

    GameObject go = GameObject.Find("MapBorder");
    if (go == null)
    {
      go = new GameObject { name = $"MapBorder" };
      collider = go.GetOrAddComponent<PolygonCollider2D>();
      Vector2[] points = new Vector2[4];

      points[0] = new Vector2(-width / 2, height / 2); //좌상
      points[1] = new Vector2(-width / 2, -height / 2); //좌하
      points[2] = new Vector2(width / 2, -height / 2); //우하
      points[3] = new Vector2(width / 2, height / 2); //우상
      
      collider.points = points;
    }
    else collider = go.GetComponent<PolygonCollider2D>();
    collider.isTrigger = true;

    return collider;
  }

  private void ParseCollisionData(GameObject map, string mapName, string tilemap = "Tilemap_Collision")
  {
    GameObject collision = Util.FindChild(map, tilemap, true);
    if (collision != null)
      collision.SetActive(false);

    TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}Collision");
    StringReader reader = new StringReader(txt.text);

    MinX = int.Parse(reader.ReadLine());
    MaxX = int.Parse(reader.ReadLine());
    MinY = int.Parse(reader.ReadLine());
    MaxY = int.Parse(reader.ReadLine());

    int xCount = MaxX - MinX + 1;
    int yCount = MaxY - MinY + 1;
    _collision = new FCellCollisionTypes[xCount, yCount];

    for (int y = 0; y < yCount; y++)
    {
      string line = reader.ReadLine();
      for (int x = 0; x < xCount; x++)
      {
        switch (line[x])
        {
          case Define.MAP_TOOL_WALL:
            _collision[x, y] = FCellCollisionTypes.Wall;
            break;
          case Define.MAP_TOOL_NONE:
            _collision[x, y] = FCellCollisionTypes.None;
            break;
          case Define.MAP_TOOL_SEMI_WALL:
            _collision[x, y] = FCellCollisionTypes.SemiWall;
            break;
        }
      }
    }
  }

  public bool MoveTo(Creature obj, Vector3Int cellPos, bool forceMove = false)
  {
    if (CanGo(obj, cellPos) == false)
      return false;

    RemoveObject(obj);

    AddObject(obj, cellPos);

    obj.SetCellPos(cellPos, forceMove);

    //Debug.Log($"Move To {cellPos}");

    return true;
  }

  #region Helpers

  public BaseObject GetObject(Vector3Int cellPos)
  {

    _cells.TryGetValue(cellPos, out BaseObject value);
    return value;
  }

  public BaseObject GetObject(Vector3 worldPos)
  {
    Vector3Int cellPos = World2Cell(worldPos);
    return GetObject(cellPos);
  }

  private bool RemoveObject(BaseObject obj)
  {
    BaseObject prev = GetObject(obj.CellPos);

    if(prev != obj)
    {
      return false;
    }

    _cells[obj.CellPos] = null;
    return true;
  }

  private bool AddObject(BaseObject obj, Vector3Int cellPos)
  {
    if (CanGo(cellPos) == false)
    {
      Debug.LogWarning($"AddObject Failed : {obj.name}");
      return false;
    }

    BaseObject prev = GetObject(cellPos);
    if(prev != null)
    {
      Debug.LogWarning($"AddObject Failed : Another Object");
    }

    _cells[cellPos] = obj;
    return true;
  }

  public bool CanGo(BaseObject self, Vector3 worldPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
  {
    return CanGo(self, World2Cell(worldPos), ignoreObjects, ignoreSemiWall);
  }

  public bool CanGo(Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
  {
    if (cellPos.x < MinX || cellPos.x > MaxX)
      return false;
    if (cellPos.y < MinY || cellPos.y > MaxY)
      return false;

    if(ignoreObjects == false)
    {
      BaseObject obj = GetObject(cellPos);
      if (obj != null)
        return false;
    }

    int x = cellPos.x - MinX;
    int y = MaxY - cellPos.y;
    FCellCollisionTypes type = _collision[x, y];
    if (type == FCellCollisionTypes.None)
      return true;

    if (ignoreSemiWall && type == FCellCollisionTypes.SemiWall)
      return true;

    return false;
  }

  public void ClearObjects()
  {
    _cells.Clear();
  }

  #endregion
}
