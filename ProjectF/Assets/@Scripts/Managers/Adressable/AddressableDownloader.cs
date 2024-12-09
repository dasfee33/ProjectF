using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Util;
using static Define;
using UnityEngine;

public class AddressableDownloader
{
  public static string DownloadURL = "https://2746801c-de23-4a66-b822-a8fa07a50254.client-api.unity3dusercontent.com/client_api/v1/environments/production/buckets/0dcf2acd-e042-4fe7-a82f-175500d818dc/releases/b90dfeb6-3369-4284-a99c-d00bf6a2d4dc/entry_by_path/content/?path=";
  DownloadEvent Events;

  string LabelToDownload;

  public long TotalSize;
  AsyncOperationHandle DownloadHandle;

  public DownloadEvent InitializeSystem(string label, string downloadURL)
  {
    Debug.Log("InitializeSystem");
    //네트워크 끊김 감지
    if (IsNetworkValid() == false)
    {

    }

    Events = new DownloadEvent();

    Addressables.InitializeAsync().Completed += OnInitialized;

    LabelToDownload = label;
    DownloadURL = downloadURL;

    ResourceManager.ExceptionHandler += OnException;

    return Events;
  }

  public void Update()
  {
    //네트워크 끊김 감지
    if(IsNetworkValid() == false)
    {

    }

    if(DownloadHandle.IsValid()
      && !DownloadHandle.IsDone
      && DownloadHandle.Status != AsyncOperationStatus.Failed)
    {
      var status = DownloadHandle.GetDownloadStatus();

      long curDownloadedSize = status.DownloadedBytes;
      long remainSize = TotalSize - curDownloadedSize;

      Events.NotifyDownloadProgress(
        new DownloadProgressStatus(
          status.DownloadedBytes
          , TotalSize
          , remainSize
          , status.Percent));
    }
  }

  public void UpdateCatalog()
  {
    Debug.Log("UpdateCatalog");
    Addressables.CheckForCatalogUpdates().Completed += (result) =>
    {
      var catalogToUpdate = result.Result;

      if (catalogToUpdate.Count > 0)
      {
        Addressables.UpdateCatalogs(catalogToUpdate).Completed += OnCatalogUpdate;
      }
      else Events.NotifyCatalogUpdate();
    };
  }

  public void DownloadSize()
  {
    Debug.Log("DownLoadSize");
    Addressables.GetDownloadSizeAsync(LabelToDownload).Completed += handle =>
    {
      if (handle.Status == AsyncOperationStatus.Succeeded)
      {
        long downloadSize = handle.Result;
        OnSizeDownloaded(handle);
      }
      else
      {
        Debug.LogError($"Failed to DownloadSize for Label : {LabelToDownload}");
      }
    };
  }

  public void StartDownload()
  {
    Debug.Log("StartDownLoad");
    Addressables.DownloadDependenciesAsync(LabelToDownload).Completed += OnDependenciesDownloaded;
  }

  public void ResourceListGenerated()
  {
    Debug.Log("ResourceListGenerated");

    //다운로드가 완료되면 리소스 로케이션을 먼저 로드
    Addressables.LoadResourceLocationsAsync(LabelToDownload, typeof(UnityEngine.Object)).Completed += op =>
    {
      if (op.Status == AsyncOperationStatus.Succeeded)
      {
        float loadCount = 0;
        float totalCount = op.Result.Count;

        // 로드된 리소스 위치 정보를 순회
        foreach (var location in op.Result)
        {
          Debug.Log(location.PrimaryKey);
          // 각 위치에서 에셋을 비동기적으로 로드
          Addressables.LoadAssetAsync<UnityEngine.Object>(location.PrimaryKey).Completed += op =>
          {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
              var key = location.PrimaryKey;
              // 로드된 에셋을 딕셔너리에 primary key와 함께 추가
              if (!Managers.Resource._resources.ContainsKey(key))
                Managers.Resource._resources.Add(location.PrimaryKey, op.Result);

              loadCount++;
              Events.NotifyResourceListGenerate(
                new ResourceProgrerssStatus(
                  loadCount,
                  totalCount
                  ));
              if(loadCount == totalCount)
              {
                OnResourceGenerateFinished();
              }
            }
            else
            {
              Debug.LogError($"Failed to load asset at {location.PrimaryKey}");
            }
          };
        }
      }
      else
      {
        Debug.LogError("Failed to load resource locations.");
      }
    };
  }


  //------------------------------------------------------------------------------------

  private void OnInitialized(AsyncOperationHandle<IResourceLocator> result)
  {
    Events.NotifyInitialized();
  }

  private void OnCatalogUpdate(AsyncOperationHandle<List<IResourceLocator>> obj)
  {
    Events.NotifyCatalogUpdate();
  }

  private void OnSizeDownloaded(AsyncOperationHandle<long> result)
  {
    TotalSize = result.Result;
    if(TotalSize <= 0)
    {
      //ResourceListGenerated();
    }
    Events.NotifySizeDownload(result.Result);

  }

  private void OnDependenciesDownloaded(AsyncOperationHandle result)
  {
    Events.NotifyDownloadFinished(result.Status == AsyncOperationStatus.Succeeded);
  }

  private void OnResourceGenerateFinished()
  {
    Events.NotifyResourceListGenerateFinished();
  }

  private void OnException(AsyncOperationHandle handle, Exception exp)
  {
    //Debug.LogError("customexceptioncaught !! " + exp.Message + "/" + handle);

    //if (exp is UnityEngine.ResourceManagement.Exceptions.RemoteProviderException)
    //{
    //  //remote 관련 에러 발생시 
    //}
  }
}
