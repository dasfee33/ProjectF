using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{
#if UNITY_EDITOR
  // % (Ctrl), # (Shift), & (Alt)
  [MenuItem("Tools/GenerateMap %#m")]
  private static void GenerateCollisionMap()
  {
    GameObject[] gameObjects = Selection.gameObjects;

    foreach (GameObject go in gameObjects)
    {
      Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

      using (var writer = File.CreateText($"Assets/@Resources/Data/MapData/{go.name}Collision.txt"))
      {
        writer.WriteLine(tm.cellBounds.xMin);
        writer.WriteLine(tm.cellBounds.xMax);
        writer.WriteLine(tm.cellBounds.yMin);
        writer.WriteLine(tm.cellBounds.yMax);

        for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
        {
          for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
          {
            TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
            if (tile != null)
            {
              if (tile.name.Contains("O"))
                writer.Write(Define.MAP_TOOL_NONE);
              else
                writer.Write(Define.MAP_TOOL_SEMI_WALL);
            }
            else
              writer.Write(Define.MAP_TOOL_WALL);
          }
          writer.WriteLine();
        }
      }
    }

    Debug.Log("Map Collision Generation Complete");
  }

  [MenuItem("Tools/GenerateMap_Obj %#o")]
  private static void GenerateObjCollisionMap()
  {
    GameObject[] gameObjects = Selection.gameObjects;

    foreach (GameObject go in gameObjects)
    {
      Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision_Obj", true);

      using (var writer = File.CreateText($"Assets/@Resources/Data/MapData/{go.name}Collision_Obj.txt"))
      {
        writer.WriteLine(tm.cellBounds.xMin);
        writer.WriteLine(tm.cellBounds.xMax);
        writer.WriteLine(tm.cellBounds.yMin);
        writer.WriteLine(tm.cellBounds.yMax);

        for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
        {
          for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
          {
            TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
            if (tile != null)
            {
              if (tile.name.Equals("Dev_objTree"))
                writer.Write(Define.MAPOBJ_TOOL_TREE);
              else if (tile.name.Equals("Dev_objRock"))
                writer.Write(Define.MAPOBJ_TOOL_ROCK);
              else if (tile.name.Equals("Dev_objGayser"))
                writer.Write(Define.MAPOBJ_TOOL_GAYSER);
              else if (tile.name.Equals("Dev_objKick"))
                writer.Write(Define.MAPOBJ_TOOL_KICKPLANT);
              else writer.Write(Define.MAPOBJ_TOOL_SEMI_WALL);
            }
            else
              writer.Write(Define.MAPOBJ_TOOL_WALL);
          }
          writer.WriteLine();
        }
      }
    }

    Debug.Log("Map Collision_Obj Generation Complete");
  }

  private static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
  {
    TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/@Resources/Data/JsonData/{path}.json");
    return JsonConvert.DeserializeObject<Loader>(textAsset.text);
  }
#endif
}
