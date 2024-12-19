using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class InputManager
{
  public InputSystem inputSystem;
  public event Action<Vector2, float> startTouch;
  public event Action<Vector2, float> endTouch;
  public event Action<BaseObject> touchObject;
  public event Action nonTouchObject;

  public event Action<Vector2> onDragging;

  public Action UIEvent;
  public Action NUIEvent;
  private bool uiFlag = false;
  //public Vector2 OnDragging
  //{
  //  get { onDragging?.Invoke(inputSystem.Touch.TouchPosition.ReadValue<Vector2>()); }
  //}

  public void Init()
  {
    inputSystem = new InputSystem();
    inputSystem.Enable();

    inputSystem.Touch.TouchPress.started += context => StartTouch(context);
    inputSystem.Touch.TouchPress.canceled += context => EndTouch(context);

    UIEvent += () => { uiFlag = true; };
    NUIEvent += () => { uiFlag = false; };
  }

  private void StartTouch(InputAction.CallbackContext context)
  {
    if (uiFlag) return;

    Vector2 touchPos = inputSystem.Touch.TouchPosition.ReadValue<Vector2>();
    TriggerAtTouchPos(touchPos);

    if (startTouch != null)
      startTouch.Invoke(inputSystem.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
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
      endTouch.Invoke(inputSystem.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
  }

  public void OnDragging()
  {
    if (uiFlag) return;

    if (onDragging != null)
      onDragging.Invoke(inputSystem.Touch.TouchPosition.ReadValue<Vector2>());
  }
}
