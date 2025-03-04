using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Managers : MonoBehaviour
{
  private static Managers s_instance;
  private static Managers Instance { get { Init(); return s_instance; } }

  #region Core
  private BackendManager _backend = new BackendManager();
  private BackendLogin _backendLogin = new BackendLogin();
  private DataManager _data = new DataManager();
  private PoolManager _pool = new PoolManager();
  private FResourceManager _resource = new FResourceManager();
  private SoundManager _sound = new SoundManager();
  private FSceneManager _scene = new FSceneManager();
  private UIManager _ui = new UIManager();
  //private InputManager _input = new InputManager();
  //private PlayerInput _playerInput => GetComponent<PlayerInput>();

  public static BackendManager BackEnd { get { return Instance?._backend; } }
  public static BackendLogin BackendLogin { get { return Instance?._backendLogin; } }
  public static DataManager Data { get { return Instance?._data; } }
  public static PoolManager Pool { get { return Instance?._pool; } }
  public static FResourceManager Resource { get { return Instance?._resource; } }
  public static SoundManager Sound { get { return Instance?._sound; } }
  public static FSceneManager Scene { get { return Instance?._scene; } }
  public static UIManager UI { get { return Instance?._ui; } }
  //public static InputManager FInput { get { return Instance?._input; } }
  //public static PlayerInput PlayerInput { get { return Instance?._playerInput; } }

  #endregion

  #region Contents
  private GameManager _game = new GameManager();
  private ObjectManager _object = new ObjectManager();
  private MapManager _map = new MapManager();
  private ToolManager _tool = new ToolManager();
  private PartitionManager _partition = new PartitionManager();
  private RandomSeedGenerate _randomSeedGenerate = new RandomSeedGenerate();
  private GameDayManager _gameDay = new GameDayManager();
  private EventManager _event = new EventManager();

  public static GameManager Game { get { return Instance?._game; } }
  public static ObjectManager Object { get { return Instance?._object; } }
  public static MapManager Map { get { return Instance?._map; } }
  public static ToolManager Tool { get { return Instance?._tool; } }
  public static PartitionManager Partition { get { return Instance?._partition; } }
  public static RandomSeedGenerate RandomSeedGenerate { get { return Instance?._randomSeedGenerate; } }
  public static GameDayManager GameDay { get { return Instance?._gameDay; } }
  public static EventManager Event { get { return Instance?._event; } }

  #endregion

  public static void Init()
  {
    if(s_instance == null)
    {
      GameObject go = GameObject.Find("@Managers");
      GameObject go2 = GameObject.Find("@GameDayLight");
      GameObject go3 = GameObject.Find("@InputManager");
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
      if (go3 == null)
      {
        go3 = new GameObject { name = "@InputManager" };
        go3.AddComponent<PlayerInput>();
        go3.AddComponent<InputManager>();
      }

      DontDestroyOnLoad(go);
      DontDestroyOnLoad(go2);
      DontDestroyOnLoad(go3);

      s_instance = go.GetComponent<Managers>();
      s_instance._backend.BackendSetup();

      inputManager = go3.GetComponent<InputManager>();
    }
  }

  private static Light2D gameDayLight;
  public static Light2D GameDayLight { get { return gameDayLight; } }

  private static InputManager inputManager;
  public static InputManager FInput { get { return inputManager; } }

  private void Start()
  {
    //gameDayLight = GameObject.Find("@GameDayLight").GetComponent<Light2D>();
    //gameDayLight.lightType = Light2D.LightType.Global;
    StartCoroutine(Instance?._gameDay.coDay());
  }

  private void Update()
  {
    //if(FInput.inputSystem.Touch.TouchPress.IsPressed())
    //{
    //  FInput.OnDragging(); 
    //}
  }

  private void OnApplicationQuit()
  {
    Debug.Log("게임 종료.");
    Managers.Game.UpdateGame();
  }

}
