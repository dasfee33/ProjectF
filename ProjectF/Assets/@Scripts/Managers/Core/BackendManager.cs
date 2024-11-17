using UnityEngine;
using BackEnd;

public class BackendManager
{
  public void BackendSetup()
  {
    var bro = Backend.Initialize();

    if(bro.IsSuccess())
    {
      Debug.Log($"초기화 성공 : {bro}");
    }
    else
    {
      Debug.LogError($"초기화 실패 : {bro}");
    }
  }
}
