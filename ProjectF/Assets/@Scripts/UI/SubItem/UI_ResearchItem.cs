using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Define;

public class UI_ResearchItem : UI_Base, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  public List<Data.StructureData> buildList = new List<Data.StructureData>();

  public Data.ResearchData Data { get; set; }

  private Image outLine;

  public enum Images
  {
    
  }

  public enum Objects
  {
    ResearchObject,
    ResearchStep,
  }

  public enum Texts
  {
    Name,
    Num, //TODO
  }

  public override bool Init()
  {
    if (base.Init() == false) return false;

    BindImages(typeof(Images));
    BindObjects(typeof(Objects));
    BindTexts(typeof(Texts));
    outLine = this.GetComponent<Image>();
    

    
    return true;
  }

  public void SetInfo(Data.ResearchData _data)
  {
    Data = _data;

    GetText((int)Texts.Name).text = Data.Name;
    GetText((int)Texts.Num).text = (Data.DataId - RESEARCH_START).ToString();
    foreach (var build in _data.BuildId)
    {
      buildList.Add(Managers.Data.StructDic[build]);
    }

    AddObject();
  }

  private void AddObject()
  {

    
    var trans = GetObject((int)Objects.ResearchObject).transform;
    foreach(var build in buildList)
    {
      GameObject go = Managers.Resource.Instantiate("UI_ResearchObjectItem", trans);
      go.GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>(build.Sprite);
    }
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    Debug.Log($"Click UI : {this.name}");
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    outLine.color = Color.blue;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    outLine.color = new Vector4(0, 0, 1, 0);
  }
}
