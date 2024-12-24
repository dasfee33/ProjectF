using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class InputManager : InitBase
{
  private InputSystem inputSystem;
  private PlayerInput PlayerInput;

  public event Action<Vector2, float> startTouch;
  public event Action<Vector2, float> endTouch;
  public event Action<BaseObject> touchObject;
  public event Action nonTouchObject;

  public event Action<Vector2> onDragging;

  public Action UIEvent;
  public Action NUIEvent;
  private bool uiFlag = false;


  private InputAction touchPositionAction;
  private InputAction touchPressAction;
  private InputAction touchPressPoisiton;

  public override bool Init()
  {
    if (base.Init() == false) return false;
    PlayerInput = this.GetComponent<PlayerInput>();
    inputSystem = new InputSystem();
    inputSystem.Enable();
    PlayerInput.actions = inputSystem.asset;

    touchPositionAction = PlayerInput.actions.FindAction("TouchPosition");
    touchPressAction = PlayerInput.actions.FindAction("TouchPress");
    touchPressPoisiton = PlayerInput.actions.FindAction("TouchPressPosition");

    touchPressAction.started += StartTouch;
    //touchPressAction.performed += OnDragging;
    touchPressAction.canceled += EndTouch;

    touchPressPoisiton.performed += OnDragging;
    //inputSystem = new InputSystem();
    //inputSystem.Enable();
    //
    //inputSystem.Touch.TouchPress.started += StartTouch;
    //inputSystem.Touch.TouchPress.performed += OnDragging;
    //inputSystem.Touch.TouchPress.canceled += EndTouch;
    //
    //inputSystem.Touch.TouchPosition.started += StartTouchPosition;

    UIEvent += () => { uiFlag = true; };
    NUIEvent += () => { uiFlag = false; };

    return true;
  }

  private void StartTouchPosition(InputAction.CallbackContext context)
  {
    Debug.Log(context.ReadValue<Vector2>());
  }

  private void StartTouch(InputAction.CallbackContext context)
  {
    if (uiFlag) return;

    Vector2 touchPos = touchPositionAction.ReadValue<Vector2>();
    TriggerAtTouchPos(touchPos);

    if (startTouch != null)
      startTouch.Invoke(touchPos, (float)context.startTime);
  }

  private void TriggerAtTouchPos(Vector2 screenPos)
  {
    Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
    worldPos.z = 0f;

    RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

    if(hit.collider != null && hit.collider.isTrigger)
    {
      var scr = hit.collider.gameObject.GetComponent<BaseObject>();
      if (scr != null)
      {
        touchObject.Invoke(scr);
      }
      //else nonTouchObject.Invoke();
    }
  }

  private void EndTouch(InputAction.CallbackContext context)
  {
    if (uiFlag) uiFlag = !uiFlag;

    if (endTouch != null)
      endTouch.Invoke(touchPositionAction.ReadValue<Vector2>(), (float)context.time);
  }

  public void OnDragging(InputAction.CallbackContext context)
  {
    Debug.Log(context.ReadValue<Vector2>());
    if (uiFlag) return;

    if (onDragging != null)
      onDragging.Invoke(context.ReadValue<Vector2>());
  }
}
