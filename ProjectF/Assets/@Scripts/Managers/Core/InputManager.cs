using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class InputManager
{
  public InputSystem inputSystem;
  public event Action<Vector2, float> startTouch;
  public event Action<Vector2, float> endTouch;

  public event Action<Vector2> onDragging;
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
  }

  private void StartTouch(InputAction.CallbackContext context)
  {
    if (startTouch != null)
      startTouch.Invoke(inputSystem.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
  }

  private void EndTouch(InputAction.CallbackContext context)
  {
    if (endTouch != null)
      endTouch.Invoke(inputSystem.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
  }

  public void OnDragging()
  {
    if (onDragging != null)
      onDragging.Invoke(inputSystem.Touch.TouchPosition.ReadValue<Vector2>());
  }
}
