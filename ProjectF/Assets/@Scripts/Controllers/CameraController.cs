using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : InitBase
{
  private BaseObject _target;
  public CinemachineBrain cam;
  public CinemachineVirtualCamera virtualCam;
  public CinemachineConfiner2D confinerCam;
  public BaseObject Target
  {
    get { return _target; }
    set { _target = value; }
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    //Camera.main.orthographicSize = 2.92f;
    cam = this.GetComponent<CinemachineBrain>();
    virtualCam = cam.GetComponentInChildren<CinemachineVirtualCamera>();
    confinerCam = cam.GetComponentInChildren<CinemachineConfiner2D>();

    virtualCam.m_Lens.OrthographicSize = 3.4f;

    return true;
  }

  private void LateUpdate()
  {
    //if(Target == null) return;

    //Vector3 targetPos = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10);
    //transform.position = targetPos;
  }
}
