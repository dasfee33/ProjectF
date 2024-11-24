using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

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

  public float defaultSize;
  public float focusSize = 1.5f;
  public float zoomDuration = 0.5f;

  //private Vector2 startPos;
  //private Vector2 curPos;

  public int targetNum = 0;

  public override bool Init()
  {
    if (base.Init() == false) return false;

    //Camera.main.orthographicSize = 2.92f;
    cam = this.GetComponent<CinemachineBrain>();
    virtualCam = cam.GetComponentInChildren<CinemachineVirtualCamera>();
    confinerCam = cam.GetComponentInChildren<CinemachineConfiner2D>();

    virtualCam.m_Lens.OrthographicSize = 3.4f;
    //virtualCam.m_Lens.OrthographicSize = 5.5f;

    defaultSize = virtualCam.m_Lens.OrthographicSize;

    //Managers.FInput.startTouch -= StartTouch;
    //Managers.FInput.startTouch += StartTouch;

    //Managers.FInput.onDragging -= OnDragging;
    //Managers.FInput.onDragging += OnDragging;

    //Managers.FInput.endTouch -= EndTouch;
    //Managers.FInput.endTouch += EndTouch;

    Managers.FInput.touchObject -= FocusObject;
    Managers.FInput.touchObject += FocusObject;

    Managers.FInput.nonTouchObject -= NonFocusObject;
    Managers.FInput.nonTouchObject += NonFocusObject;



    return true;
  }

  //private bool isDragging = false;
  //private float moveRate = 1.5f;
  //private void StartTouch(Vector2 pos, float time)
  //{
  //  startPos = pos;
  //  isDragging = true;

  //  FollowTarget.position = Camera.main.ScreenToWorldPoint(pos);

  //  virtualCam.Follow = FollowTarget;
  //}

  //private void OnDragging(Vector2 pos)
  //{
  //  if(isDragging)
  //  {
  //    Vector3 targetPosition = Camera.main.ScreenToViewportPoint(pos);
  //    FollowTarget.position = targetPosition * moveRate;
  //  }
  //}

  //private void EndTouch(Vector2 pos, float time)
  //{
  //  isDragging = false;
  //  virtualCam.Follow = null;
  //}

  private Transform saveFollow;

  private void FocusObject(BaseObject obj)
  {
    if (obj != null)
    {
      virtualCam.m_Lens.OrthographicSize = defaultSize;
      DOTween.To(() => virtualCam.m_Lens.OrthographicSize, x => virtualCam.m_Lens.OrthographicSize = x, focusSize, zoomDuration).
        SetEase(Ease.OutCubic).OnComplete(() =>
        {
          saveFollow = virtualCam.Follow;
          virtualCam.Follow = obj.transform;
        });
    }
  }

  private void NonFocusObject()
  {
    if (virtualCam.m_Lens.OrthographicSize == defaultSize) return;

    DOTween.To(() => virtualCam.m_Lens.OrthographicSize, x => virtualCam.m_Lens.OrthographicSize = x, defaultSize, zoomDuration).
        SetEase(Ease.InCubic).OnComplete(() => { virtualCam.Follow = saveFollow; });
  }

  private void LateUpdate()
  {
    //if(Target == null) return;

    //Vector3 targetPos = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10);
    //transform.position = targetPos;
  }
}
