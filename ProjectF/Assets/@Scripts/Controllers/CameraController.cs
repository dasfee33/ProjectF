using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : InitBase
{
  private BaseObject _target;
  public BaseObject Target
  {
    get { return _target; }
    set { _target = value; }
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    Camera.main.orthographicSize = 2.92f;

    return true;
  }

  private void LateUpdate()
  {
    if(Target == null) return;

    Vector3 targetPos = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10);
    transform.position = targetPos;
  }
}
