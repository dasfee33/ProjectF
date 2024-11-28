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
      Test();
    }
    else
    {
      Debug.LogError($"초기화 실패 : {bro}");
    }
  }
   
  private void Test()
  {
    Managers.BackendLogin.CustomLogin("kys", "1234");
    //Managers.Game.GameDataInsert();

    Debug.Log("테스트 종료");
  }
}
