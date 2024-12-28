using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
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

  private InputAction touchAction;
  private InputAction touchPositionAction;
  private InputAction touchPressAction;
  private InputAction touchPressPosition;

  public override bool Init()
  {
    if (base.Init() == false) return false;
    PlayerInput = this.GetComponent<PlayerInput>();
    inputSystem = new InputSystem();
    inputSystem.Enable();
    PlayerInput.actions = inputSystem.asset;

    touchAction = PlayerInput.actions.FindAction("Touch");
    touchPositionAction = PlayerInput.actions.FindAction("TouchPosition");
    touchPressAction = PlayerInput.actions.FindAction("TouchPress");
    touchPressPosition = PlayerInput.actions.FindAction("TouchPressPosition");

    //touchAction.started += StartTouch;
    //touchAction.canceled += EndTouch;
    touchPressAction.started += StartTouch;
    //touchPressAction.performed += OnDragging;
    touchPressAction.canceled += EndTouch;

    touchPressPosition.performed += OnDragging;

    //touchPressPosition.performed += OnDragging;
    
    //inputSystem = new InputSystem();
    //inputSystem.Enable();
    //
    //inputSystem.Touch.TouchPress.started += StartTouch;
    //inputSystem.Touch.TouchPress.performed += OnDragging;
    //inputSystem.Touch.TouchPress.canceled += EndTouch;
    //
    //inputSystem.Touch.TouchPosition.started += StartTouchPosition;

    return true;
  }

  //FIXME UI 터치 구분 필요
  private bool IsTouchOverUI()
  {
    //int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();

    return EventSystem.current.IsPointerOverGameObject(/*touchId*/);
  }

  private void StartTouchPosition(InputAction.CallbackContext context)
  {
    Debug.Log(context.ReadValue<Vector2>());
  }

  private void StartTouch(InputAction.CallbackContext context)
  {
    if (IsTouchOverUI()) return;

    Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();

    if (!Camera.main.pixelRect.Contains(touchPos)) return;

    TriggerAtTouchPos(touchPos);

    if (startTouch != null)
      startTouch.Invoke(touchPos, (float)context.startTime);
  }

  private void TriggerAtTouchPos(Vector2 screenPos)
  {
    Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
    worldPos.z = 0f;

    Collider2D[] hitColliders = Physics2D.OverlapPointAll(worldPos);

    if (hitColliders is null || hitColliders.Length <= 0) return;
    foreach(var hitCollider in hitColliders)
    {
      if(hitCollider.isTrigger)
      {
        var scr = hitCollider.gameObject.GetComponent<BaseObject>();
        if (scr != null)
        {
          touchObject.Invoke(scr);
          return;
        }
      }
    }

    //if(hitCollider != null && hitCollider.isTrigger)
    //{
      
    //  //else nonTouchObject.Invoke();
    //}
  }

  private void EndTouch(InputAction.CallbackContext context)
  {
    if (IsTouchOverUI()) return;

    if (endTouch != null)
    {
      Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
      if (!Camera.main.pixelRect.Contains(touchPos)) return;

      endTouch.Invoke(touchPos, (float)context.time);
    }
  }

  public void OnDragging(InputAction.CallbackContext context)
  {
    if (IsTouchOverUI()) return;

    var touchPos = context.ReadValue<Vector2>();
    if (!Camera.main.pixelRect.Contains(touchPos)) return;

    if (onDragging != null)
      onDragging.Invoke(touchPos);
  }
}
