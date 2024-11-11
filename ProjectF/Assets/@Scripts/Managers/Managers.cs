using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
  private InputManager _input = new InputManager();

  public static DataManager Data { get { return Instance?._data; } }
  public static PoolManager Pool { get { return Instance?._pool; } }
  public static ResourceManager Resource { get { return Instance?._resource; } }
  public static SoundManager Sound { get { return Instance?._sound; } }
  public static FSceneManager Scene { get { return Instance?._scene; } }
  public static UIManager UI { get { return Instance?._ui; } }
  public static InputManager FInput { get { return Instance?._input; } }

  #endregion

  #region Contents
  private GameManager _game = new GameManager();
  private ObjectManager _object = new ObjectManager();
  private MapManager _map = new MapManager();
  private ToolManager _tool = new ToolManager();
  private PartitionManager _partition = new PartitionManager();
  private RandomSeedGenerate _randomSeedGenerate = new RandomSeedGenerate();
  private GameDayManager _gameDay = new GameDayManager();

  public static GameManager Game { get { return Instance?._game; } }
  public static ObjectManager Object { get { return Instance?._object; } }
  public static MapManager Map { get { return Instance?._map; } }
  public static ToolManager Tool { get { return Instance?._tool; } }
  public static PartitionManager Partition { get { return Instance?._partition; } }
  public static RandomSeedGenerate RandomSeedGenerate { get { return Instance?._randomSeedGenerate; } }
  public static GameDayManager GameDay { get { return Instance?._gameDay; } }

  #endregion

  public static void Init()
  {
    if(s_instance == null)
    {
      GameObject go = GameObject.Find("@Managers");
      GameObject go2 = GameObject.Find("@GameDayLight");
      if(go == null)
      {
        go = new GameObject { name = "@Managers" };
        go.AddComponent<Managers>();
      }
      if(go2 == null)
      {
        go2 = new GameObject { name = "@GameDayLight" };
        go2.AddComponent<Light2D>();
      }

      DontDestroyOnLoad(go);
      DontDestroyOnLoad(go2);

      s_instance = go.GetComponent<Managers>();
    }
  }

  private static Light2D gameDayLight;
  public static Light2D GameDayLight { get { return gameDayLight; } }

  private void Start()
  {
    gameDayLight = GameObject.Find("@GameDayLight").GetComponent<Light2D>();
    gameDayLight.lightType = Light2D.LightType.Global;
    StartCoroutine(Instance?._gameDay.coDay());

    
  }

  private bool isInputProcessed = false;

  #region Input
  private void Update()
  {
    if (Input.touchCount > 0)
    {
      if (Input.touchCount == 1)
      {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
          case TouchPhase.Began:
            if (!isInputProcessed)
            {
              isInputProcessed = true;
              FInput.startPos = touch.position;
            }
            break;
          case TouchPhase.Moved:
            if(isInputProcessed)
            {
              FInput.curPos = touch.position;
              FInput.OnDrag?.Invoke(FInput.startPos, FInput.curPos, Vector3.Distance(FInput.startPos, FInput.curPos));
              FInput.HandleDrag();
            }
            break;
          case TouchPhase.Ended:
            if(isInputProcessed)
            {
              FInput.OnTouch?.Invoke(touch.position);
              FInput.HandleClick();
              isInputProcessed = false;
            }
            
            break;
        }
      }
    }
    else if (Input.touchCount == 2)
    {
      Touch touch1 = Input.GetTouch(0);
      Touch touch2 = Input.GetTouch(1);

      if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
      {
        FInput.curDistance = Vector3.Distance(touch1.position, touch2.position);

        if (FInput.startDistance <= 0f)
        {
          FInput.startDistance = FInput.curDistance;
        }
        else
        {
          float diff = FInput.curDistance - FInput.startDistance;
          if (diff > 0)
          {
            FInput.ZoonIn?.Invoke();
            FInput.HandleZoomIn();
          }
          else if (diff < 0)
          {
            FInput.ZoomOut?.Invoke();
            FInput.HandleZoomOut();
          }
        }
      }

      if (touch1.phase == TouchPhase.Ended && touch2.phase == TouchPhase.Ended)
      {
        FInput.startDistance = 0f;
      }
       
    }
  }
  #endregion
}
