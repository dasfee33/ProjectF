using System;
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
  public Tilemap tilemap { get; private set; }

  Dictionary<Vector3Int, BaseObject> _cells = new Dictionary<Vector3Int, BaseObject>();

  private int MinX;
  private int MaxX;
  private int MinY;
  private int MaxY;

  public Vector3Int World2Cell(Vector3 worldPos) { return CellGrid.WorldToCell(worldPos); }
  public Vector3 Cell2World(Vector3Int cellPos) { return CellGrid.CellToWorld(cellPos); }

  FCellCollisionTypes[,] _collision;
  FCellCollisionTypes[,] _collision_obj;

  public FCellCollisionTypes[,] Collision { get { return _collision; } private set { _collision = value; } }
  public FCellCollisionTypes[,] Collision_obj { get { return _collision_obj; } private set { _collision_obj = value; } }

  public void LoadMap(string mapName)
  {
    DestroyMap();

    GameObject map = Managers.Resource.Instantiate(mapName);
    map.transform.position = Vector3.zero;
    map.name = $"@Map_{mapName}";

    Map = map;
    MapName = mapName;
    CellGrid = map.GetComponent<Grid>();

    _collision = ParseCollisionData(map, mapName);
    _collision_obj = ParseCollisionData(map, mapName, "Tilemap_Collision_Obj");
    
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
    tilemap = map.transform.GetChild(0).GetComponent<Tilemap>();
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

  private FCellCollisionTypes[,] ParseCollisionData(GameObject map, string mapName, string tilemap = "Tilemap_Collision")
  {
    GameObject collision = Util.FindChild(map, tilemap, true);
    if (collision != null)
      collision.SetActive(false);

    string mapDataAsset = $"{mapName}Collision";
    if(tilemap != default)
    {
      switch(tilemap)
      {
        case "Tilemap_Collision_Obj":
          mapDataAsset = $"{mapName}Collision_Obj";
          break;
        //TODO ADD
      }
    }

    TextAsset txt = Managers.Resource.Load<TextAsset>(mapDataAsset);
    StringReader reader = new StringReader(txt.text);

    MinX = int.Parse(reader.ReadLine());
    MaxX = int.Parse(reader.ReadLine());
    MinY = int.Parse(reader.ReadLine());
    MaxY = int.Parse(reader.ReadLine());

    int xCount = MaxX - MinX + 1;
    int yCount = MaxY - MinY + 1;
    FCellCollisionTypes[,] collisionArray = new FCellCollisionTypes[xCount, yCount];

    for (int y = 0; y < yCount; y++)
    {
      string line = reader.ReadLine();
      for (int x = 0; x < xCount; x++)
      {
        switch (line[x])
        {
          case Define.MAP_TOOL_WALL:
            collisionArray[x, y] = FCellCollisionTypes.Wall;
            break;
          case Define.MAP_TOOL_NONE:
            collisionArray[x, y] = FCellCollisionTypes.None;
            break;
          case Define.MAP_TOOL_SEMI_WALL:
            collisionArray[x, y] = FCellCollisionTypes.SemiWall;
            break;
        }
      }
    }

    return collisionArray;
  }

  public bool MoveTo(Creature obj, Vector3Int cellPos, bool forceMove = false)
  {
    if (CanGo(cellPos) == false)
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

  public BaseObject NearGetObject(Vector3 worldPos, Vector3Int cellPos, int depth = 1)
  {
    BaseObject obj = null;
    //Vector3Int cellPos = World2Cell(worldPos) + value;
    
    for(int x = -depth; x <= depth; x++)
    {
      for(int y = -depth; y <= depth; y++)
      {
        if (x == 0 && y == 0) continue;
        obj = GetObject(cellPos + new Vector3Int(x, y));
        if (obj != null) return obj;
      }
    }
    return obj;
  }

  private void RemoveObject(BaseObject obj)
  {
    int extraCells = 0;
    if (obj != null)
      extraCells = obj.ExtraCells;

    Vector3Int cellPos = obj.CellPos;

    for (int dx = -extraCells; dx <= extraCells; dx++)
    {
      for (int dy = -extraCells; dy <= extraCells; dy++)
      {
        Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);
        BaseObject prev = GetObject(newCellPos);

        if (prev == obj)
          _cells[newCellPos] = null;
      }
    }
  }

  public void AddObject(BaseObject obj, Vector3Int cellPos)
  {
    int extraCells = 0;
    if (obj != null)
      extraCells = obj.ExtraCells;
    else return;

    for (int dx = -extraCells; dx <= extraCells; dx++)
    {
      for (int dy = -extraCells; dy <= extraCells; dy++)
      {
        Vector3Int newCellPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy);

        BaseObject prev = GetObject(newCellPos);
        if (prev != null && prev != obj)
          Debug.LogWarning($"AddObject {obj}");

        _cells[newCellPos] = obj;
      }
    }
  }

  public FCellCollisionTypes GetTileCollisionType(Vector3Int cellPos)
  {
    var result = _collision[cellPos.x, cellPos.y];
    return result;
  }
  public FCellCollisionTypes GetTileObjCollisionType(Vector3Int cellPos)
  {
    var result = _collision_obj[cellPos.x, cellPos.y];
    return result;
  }

  public bool CanGo(Vector3 worldPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
  {
    return CanGo(World2Cell(worldPos), ignoreObjects, ignoreSemiWall);
  }

  public bool CanGo(Vector3Int cellPos, bool ignoreObjects = false, bool ignoreSemiWall = false)
  {
    if (cellPos.x < MinX || cellPos.x > MaxX)
      return false;
    if (cellPos.y < MinY || cellPos.y > MaxY)
      return false;

    if (ignoreObjects == false)
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

  #region A* PathFinding
  public struct PQNode : IComparable<PQNode>
  {
    public int H; // Heuristic
    public Vector3Int CellPos;
    public int Depth;

    public int CompareTo(PQNode other)
    {
      if (H == other.H)
        return 0;
      return H < other.H ? 1 : -1;
    }
  }

  List<Vector3Int> _delta = new List<Vector3Int>()
    {
        new Vector3Int(0, 1, 0), // U
		new Vector3Int(1, 1, 0), // UR
		new Vector3Int(1, 0, 0), // R
		new Vector3Int(1, -1, 0), // DR
		new Vector3Int(0, -1, 0), // D
		new Vector3Int(-1, -1, 0), // LD
		new Vector3Int(-1, 0, 0), // L
		new Vector3Int(-1, 1, 0), // LU
	};

  public List<Vector3Int> FindPath(BaseObject self, Vector3Int startCellPos, Vector3Int destCellPos, int maxDepth = 10)
  {
    Dictionary<Vector3Int, int> best = new Dictionary<Vector3Int, int>();
    Dictionary<Vector3Int, Vector3Int> parent = new Dictionary<Vector3Int, Vector3Int>();

    PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>(); // OpenList

    Vector3Int pos = startCellPos;
    Vector3Int dest = destCellPos;

    Vector3Int closestCellPos = startCellPos;
    int closestH = (dest - pos).sqrMagnitude;

    {
      int h = (dest - pos).sqrMagnitude;
      pq.Push(new PQNode() { H = h, CellPos = pos, Depth = 1 });
      parent[pos] = pos;
      best[pos] = h;
    }

    while (pq.Count > 0)
    {
      PQNode node = pq.Pop();
      pos = node.CellPos;

      if (pos == dest)
        break;

      if (node.Depth >= maxDepth)
        break;

      foreach (Vector3Int delta in _delta)
      {
        Vector3Int next = pos + delta;

        if (CanGo(next) == false)
          continue;

        int h = (dest - next).sqrMagnitude;

        if (best.ContainsKey(next) == false)
          best[next] = int.MaxValue;

        if (best[next] <= h)
          continue;

        best[next] = h;

        pq.Push(new PQNode() { H = h, CellPos = next, Depth = node.Depth + 1 });
        parent[next] = pos;

        if (closestH > h)
        {
          closestH = h;
          closestCellPos = next;
        }
      }
    }

    if (parent.ContainsKey(dest) == false)
      return CalcCellPathFromParent(parent, closestCellPos);

    return CalcCellPathFromParent(parent, dest);
  }

  List<Vector3Int> CalcCellPathFromParent(Dictionary<Vector3Int, Vector3Int> parent, Vector3Int dest)
  {
    List<Vector3Int> cells = new List<Vector3Int>();

    if (parent.ContainsKey(dest) == false)
      return cells;

    Vector3Int now = dest;

    while (parent[now] != now)
    {
      cells.Add(now);
      now = parent[now];
    }

    cells.Add(now);
    cells.Reverse();

    return cells;
  }

  #endregion
}
