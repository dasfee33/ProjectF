using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
  public event Action<PointerEventData> OnClickHandler = null;
  public event Action<PointerEventData> OnPointerDownHandler = null;
  public event Action<PointerEventData> OnPointerUpHandler = null;
  public event Action<PointerEventData> OnDragHandler = null;

  public void OnPointerClick(PointerEventData eventData)
  {
    OnClickHandler?.Invoke(eventData);
    //Managers.FInput.UIEvent.Invoke();
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    OnPointerDownHandler?.Invoke(eventData);
    Managers.FInput.UIEvent.Invoke();
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    OnPointerUpHandler?.Invoke(eventData);
    Managers.FInput.NUIEvent.Invoke();
  }

  public void OnDrag(PointerEventData eventData)
  {
    OnDragHandler?.Invoke(eventData);
    Managers.FInput.UIEvent.Invoke();
  }
}
