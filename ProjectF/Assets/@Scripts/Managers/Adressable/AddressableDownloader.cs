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
  public static string DownloadURL = "https://storage.cloud.google.com/ysaddressabletest2/Android/";
  DownloadEvent Events;

  string LabelToDownload;

  long TotalSize;
  AsyncOperationHandle DownloadHandle;

  public DownloadEvent InitializeSystem(string label, string downloadURL)
  {
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
    Addressables.GetDownloadSizeAsync(LabelToDownload).Completed += OnSizeDownloaded;
  }

  public void StartDownload()
  {
    DownloadHandle = Addressables.DownloadDependenciesAsync(LabelToDownload);
    DownloadHandle.Completed += OnDependenciesDownloaded;
    //DownloadHandle.Completed += ((op) =>
    //{
    //  // 다운로드가 완료되면 리소스 로케이션을 먼저 로드
    //  Addressables.LoadResourceLocationsAsync(LabelToDownload, typeof(UnityEngine.Object)).Completed += locHandle =>
    //  {
    //    if (locHandle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //      // 로드된 리소스 위치 정보를 순회
    //      foreach (var location in locHandle.Result)
    //      {
    //        // 각 위치에서 에셋을 비동기적으로 로드
    //        Addressables.LoadAssetAsync<UnityEngine.Object>(location).Completed += assetHandle =>
    //        {
    //          if (assetHandle.Status == AsyncOperationStatus.Succeeded)
    //          {
    //            // 로드된 에셋을 딕셔너리에 primary key와 함께 추가
    //            Managers.Resource._resources.Add(location.PrimaryKey, assetHandle.Result);
    //          }
    //          else
    //          {
    //            Debug.LogError($"Failed to load asset at {location.PrimaryKey}");
    //          }
    //        };
    //      }
    //    }
    //    else
    //    {
    //      Debug.LogError("Failed to load resource locations.");
    //    }
    //  };
    //});
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
    Events.NotifySizeDownload(result.Result);
  }

  private void OnDependenciesDownloaded(AsyncOperationHandle result)
  {
    Events.NotifyDownloadFinished(result.Status == AsyncOperationStatus.Succeeded);
  }

  private void OnException(AsyncOperationHandle handle, Exception exp)
  {
    Debug.LogError("customexceptioncaught !! " + exp.Message);

    if (exp is UnityEngine.ResourceManagement.Exceptions.RemoteProviderException)
    {
      //remote 관련 에러 발생시 
    }
  }
}
