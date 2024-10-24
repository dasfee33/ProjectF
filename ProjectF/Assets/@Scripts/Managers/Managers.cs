using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
  private static Managers s_instance;
  private static Managers Instance { get { Init(); return s_instance; } }

  #region Core
  private DataManager _data = new DataManager();
  private PoolManager _pool = new PoolManager();
  private ResourceManager _resource = new ResourceManager();
  private SoundManager _sound = new SoundManager();
  private FSceneManager _scene = new FSceneManager();
  private UIManager _ui = new UIManager();

  public static DataManager Data { get { return Instance?._data; } }
  public static PoolManager Pool { get { return Instance?._pool; } }
  public static ResourceManager Resource { get { return Instance?._resource; } }
  public static SoundManager Sound { get { return Instance?._sound; } }
  public static FSceneManager Scene { get { return Instance?._scene; } }
  public static UIManager UI { get { return Instance?._ui; } }

  #endregion

  #region Contents
  private GameManager _game = new GameManager();
  private ObjectManager _object = new ObjectManager();
  private MapManager _map = new MapManager();
  private ToolManager _tool = new ToolManager();
  private PartitionManager _partition = new PartitionManager();

  public static GameManager Game { get { return Instance?._game; } }
  public static ObjectManager Object { get { return Instance?._object; } }
  public static MapManager Map { get { return Instance?._map; } }
  public static ToolManager Tool { get { return Instance?._tool; } }
  public static PartitionManager Partition { get { return Instance?._partition; } }

  #endregion

  public static void Init()
  {
    if(s_instance == null)
    {
      GameObject go = GameObject.Find("@Managers");
      if(go == null)
      {
        go = new GameObject { name = "@Managers" };
        go.AddComponent<Managers>();
      }

      DontDestroyOnLoad(go);

      s_instance = go.GetComponent<Managers>();
    }
  }
}
