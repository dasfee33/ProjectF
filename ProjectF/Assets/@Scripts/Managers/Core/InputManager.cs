using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager
{
  public Action<Vector3> OnTouch;
  public Action<Vector3, Vector3, float> OnDrag;
  public Action ZoomOut;
  public Action ZoonIn;

  public Vector3 startPos;
  public Vector3 curPos;

  public float startDistance;
  public float curDistance;

  public void HandleClick()
  {

  }

  public void HandleDrag()
  {

  }

  public void HandleZoomIn()
  {

  }

  public void HandleZoomOut()
  {

  }
  
}
