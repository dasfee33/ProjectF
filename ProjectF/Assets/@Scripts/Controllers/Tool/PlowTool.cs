using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class PlowTool : MonoBehaviour
{
  public Grid grid;
  public Tilemap tilemap;
  private Tile tile;

  private Vector3Int startCellPos;
  private Vector3Int endCellPos;

  private bool showSelection;

  private List<Vector3Int> areaTiles = new List<Vector3Int>();
  private List<Vector3Int> plowTiles = new List<Vector3Int>();

  public string toolTag => SetTag("Plow");

  private void Awake()
  {
    //FIXME
    if (tile == null) tile = Managers.Resource.Load<Tile>("plowToolTile");

    SetTag(tag);
  }

  private string SetTag(string tag)
  {
    this.tag = tag;

    return this.tag;
  }

  private void Update()
  {
    Vector3 mousePos = Input.mousePosition;
    Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
    worldPos.z = 0;

    if (Input.GetMouseButtonDown(0))
    {
      startCellPos = grid.WorldToCell(worldPos);
      startCellPos.z = 0;
    }

    showSelection = Input.GetMouseButton(0);

    endCellPos = grid.WorldToCell(worldPos);
    endCellPos.z = 0;

    if (Input.GetMouseButtonUp(0))
    {
      ClearTile();
      StartCoroutine(GenerateTile());
    }
    else if (showSelection)
    {
      // 드래그 중인 영역을 타일맵에 시각적으로 표시
      UpdateSelectionTile();
    }
  }

  private void UpdateSelectionTile()
  {
  
    // 드래그 영역의 최소 및 최대 셀 좌표 계산
    int minX = Mathf.Min(startCellPos.x, endCellPos.x);
    int maxX = Mathf.Max(startCellPos.x, endCellPos.x);
    int minY = Mathf.Min(startCellPos.y, endCellPos.y);
    int maxY = Mathf.Max(startCellPos.y, endCellPos.y);

    RemovePreviousTiles(minX, maxX, minY, maxY);

    // 드래그한 영역에 타일 설정
    for (int x = minX; x <= maxX; x++)
    {
      for (int y = minY; y <= maxY; y++)
      {
        Vector3Int cellPos = new Vector3Int(x, y, 0);
        SetTile(cellPos);
        
      }
    }
  }

  private void RemovePreviousTiles(int minX, int maxX, int minY, int maxY)
  {
    // 이전에 추가된 타일을 제거
    foreach (var pos in areaTiles)
    {
      // 타일이 있는 위치만 확인
      if (pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY)
      {
        tilemap.SetTile(pos, null); // 타일 제거
      }
    }

    // 리스트에서 제거된 타일의 위치 삭제
    areaTiles.RemoveAll(pos => pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY);
  }


  private List<Vector3Int> SetTile(Vector3Int pos)
  {
    if (tilemap.GetTile(pos) == null)
    {
      //현재 타일의 좌표를 월드로 바꾸고 월드맵의 컬리젼 셀좌표로 바꿈
      var worldPos = tilemap.CellToWorld(pos);
      var tileCollision = Managers.Map.GetTileCollisionType(worldPos);
      var other = Managers.Map.GetObject(worldPos);
      if (tileCollision == FCellCollisionTypes.Wall || tileCollision == FCellCollisionTypes.SemiWall || other != null)
      {
        tile.color = Color.red;
      }
      else
      {
        tile.color = Color.white;
      }
      tilemap.SetTile(pos, tile);
      areaTiles.Add(pos);
    }
    
    return areaTiles;
  }

  private void ClearTile()
  {
    if (areaTiles.Count <= 0) return;

    foreach(Vector3Int pos in areaTiles)
    {
      if(tilemap.GetTile(pos) != null)
      {
        tilemap.SetTile(pos, null);
      }
    }

    areaTiles.Clear();
  }

  private IEnumerator GenerateTile()
  {
    tile.color = Color.white;
    Queue<Vector3Int> queue = new Queue<Vector3Int>();
    HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

    // 시작 위치를 큐에 추가
    Vector3Int startPos = new Vector3Int(startCellPos.x, startCellPos.y, 0);
    queue.Enqueue(startPos);
    visited.Add(startPos);

    // 드래그 영역의 최대 범위 계산
    int minX = Mathf.Min(startCellPos.x, endCellPos.x);
    int maxX = Mathf.Max(startCellPos.x, endCellPos.x);
    int minY = Mathf.Min(startCellPos.y, endCellPos.y);
    int maxY = Mathf.Max(startCellPos.y, endCellPos.y);

    //BFS
    while (queue.Count > 0)
    {
      Vector3Int current = queue.Dequeue();

      // 현재 위치가 드래그 영역 내인지 확인
      if (current.x >= minX && current.x <= maxX && current.y >= minY && current.y <= maxY)
      {
        var worldPos = tilemap.CellToWorld(current);
        var colType = Managers.Map.GetTileCollisionType(worldPos);
        var other = Managers.Map.GetObject(worldPos);
        if (colType == FCellCollisionTypes.SemiWall || colType == FCellCollisionTypes.Wall || other != null)
          continue;
        // 현재 위치에 타일 설정
        if (tilemap.GetTile(current) == null)
        {
          tilemap.SetTile(current, tile);
          plowTiles.Add(current);
          yield return new WaitForSeconds(0.005f); // 설정 후 딜레이
        }

        // 인접한 타일 좌표 계산
        Vector3Int[] neighbors = {
                new Vector3Int(current.x + 1, current.y, 0),
                new Vector3Int(current.x - 1, current.y, 0),
                new Vector3Int(current.x, current.y + 1, 0),
                new Vector3Int(current.x, current.y - 1, 0)
            };

        foreach (var neighbor in neighbors)
        {
          // 큐에 추가할 조건: 방문하지 않았고, 타일이 비어있고 드래그 영역 내에 있는 경우
          if (!visited.Contains(neighbor) && tilemap.GetTile(neighbor) == null &&
              neighbor.x >= minX && neighbor.x <= maxX &&
              neighbor.y >= minY && neighbor.y <= maxY)
          {
            queue.Enqueue(neighbor);
            visited.Add(neighbor);
          }
        }
      }
    }
  }

}
