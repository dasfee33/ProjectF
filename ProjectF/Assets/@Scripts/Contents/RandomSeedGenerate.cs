using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class RandomSeedGenerate
{
  private float seed;
  private Tilemap Map;
  private int width;
  private int height;

  public void GenerateMaps()
  {
    SettingBiom(GenerateNoise());
  }

  private float[,] GenerateNoise()
  {
    seed = Random.Range(0, 1000);
    //float max = float.MaxValue;
    //float min = float.MinValue;

    if (Map == null)
    {
      Map = Managers.Map.tilemap;
      Vector3Int mapSize = Map.size;
      width = mapSize.x;
      height = mapSize.y;
    }
    float[,] noiseArr = new float[width,height];

    for(int x = 0; x < width; x++)
    {
      for(int y = 0; y < height; y++)
      {
        noiseArr[x, y] = Mathf.PerlinNoise
        (
          x * 0.1f/*Map.cellSize.x*/ + seed,
          y * 0.1f/*Map.cellSize.y*/ + seed
        );
      }
    }

    //for(int x = 0; x < height; x++)
    //{
    //  for(int y = 0; y < height; y++)
    //  {
    //    noiseArr[x, y] = Mathf.InverseLerp(min, max, noiseArr[x, y]);
    //  }
    //}

    return noiseArr;
  }


  private void SettingBiom(float[,] noiseArr)
  {
    Vector3Int point = Vector3Int.zero;

    for(int y = height; y >= 0; y--)
    {
      for(int x = 0; x <= width; x++)
      {
        var cellPos = new Vector3Int(x, y, 0);
        int X = x - width / 2;
        int Y = y - height / 2;
        FCellCollisionTypes cellType = Managers.Map.GetTileObjCollisionType(new Vector3Int(x, y));
        if (X <= -width / 2 || X >= width / 2) continue;
        if (Y <= -height / 2 || Y >= height / 2) continue;
        if (cellType == FCellCollisionTypes.Wall || cellType == FCellCollisionTypes.SemiWall) continue;
        
        point.Set(X, -Y, 0);

        var pos = Managers.Map.Cell2World(point);
        
        BaseObject bo = GetBiomObject(pos, cellPos, noiseArr[x, y]);
        Managers.Map.AddObject(bo, cellPos);
      }
    }
  }

  private BaseObject GetBiomObject(Vector3 pos, Vector3Int cellPos, float noiseValue)
  {
    BaseObject bo = Managers.Map.NearGetObject(pos, cellPos);
    if (bo != null)
      return null;
    switch(noiseValue)
    {
      case <= 0.3f:
        bo = Managers.Object.Spawn<Env>(pos, ENV_TREE_NORMAL1, "Tree1");
        break;
      case <= 0.4f:
        bo = Managers.Object.Spawn<Env>(pos, ENV_TREE_NORMAL2, "Tree2");
        break;
    }
    

    return bo;
  }
}
