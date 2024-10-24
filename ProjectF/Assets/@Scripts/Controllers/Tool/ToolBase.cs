using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolBase : InitBase, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,IDragHandler
{

  public void OnPointerClick(PointerEventData eventData)
  {
    
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    Vector3 worldPos = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);

    //Vector3Int cellPos = Grid.WorldToCell(worldPos);
  }

  public void OnPointerUp(PointerEventData eventData)
  {
  }

  public void OnDrag(PointerEventData eventData)
  {

  }
}
