using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaMotion : MonoBehaviour
{
  private Coroutine _coLaunchObject;

  public float HeightArc { get; protected set; } = 0.5f;
  public Vector3 StartPosition { get; private set; }
  public Vector3 TargetPosition { get; private set; }
  protected Action EndCallback { get; private set; }

  public float _speed;

  public void SetInfo(int dataTemplateID, Vector3 startPosition, Vector3 targetPosition, Action endCallback)
  {
    _speed = 1.0f;
    
    StartPosition = startPosition;
    TargetPosition = targetPosition;
    EndCallback = endCallback;

    //LookAtTarget = true; // TEMP

    if (_coLaunchObject != null)
      StopCoroutine(_coLaunchObject);

    _coLaunchObject = StartCoroutine(CoLaunchObject());
  }

  private IEnumerator CoLaunchObject()
  {
    float journeyLength = Vector2.Distance(StartPosition, TargetPosition);
    float totalTime = journeyLength / _speed;
    float elapsedTime = 0;

    while (elapsedTime < totalTime)
    {
      elapsedTime += Time.deltaTime;

      float normalizedTime = elapsedTime / totalTime;

      float x = Mathf.Lerp(StartPosition.x, TargetPosition.x, normalizedTime);
      float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, normalizedTime);
      float arc = HeightArc * Mathf.Sin(normalizedTime * Mathf.PI);

      float y = baseY + arc;

      var nextPos = new Vector3(x, y);

      transform.position = nextPos;

      yield return null;
    }

    EndCallback?.Invoke();
  }
}
