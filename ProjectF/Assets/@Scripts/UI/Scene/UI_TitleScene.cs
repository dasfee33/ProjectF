using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_TitleScene : UI_Scene
{
  public enum DownloadState
  {
    None = 0,

    CalculatingSize,
    NothingToDownload,

    AskingDownload,
    Downloading,
    DownloadFinished,
    ResourceGenerated,

    AllFinished,
  }

  enum Objects
  {
    Background,

    DownloadConfirm,
  }

  enum Texts
  {
    Gamestart,
    DownloadDesc,
  }

  enum Sliders
  {
    DownloadSlider,
  }

  enum Buttons
  {
    Startarea,
    Confirm,
    Cancel
  }

  private DownloadController Downloader;

  private DownloadProgressStatus progressInfo;
  private ResourceProgrerssStatus rprogressInfo;
  private FSizeUnits sizeUnit;
  private long curDownloadSizeUnit;
  private long totalSizeUnit;

  public DownloadState CurrentState { get; private set; } = DownloadState.None;


  public override bool Init()
  {
    if (base.Init() == false) return false;

    SetSafeArea(FSetUISafeArea.All);

    BindObjects(typeof(Objects));
    BindTexts(typeof(Texts));
    BindSliders(typeof(Sliders));
    BindButtons(typeof(Buttons));

    Downloader = this.GetComponent<DownloadController>();

    GetButton((int)Buttons.Confirm).gameObject.BindEvent(OnClickStartDownload, FUIEvent.Click);
    GetButton((int)Buttons.Cancel).gameObject.BindEvent(OnClickCancelDownload, FUIEvent.Click);

    //StartLoadAssets();

    return true;
  }

  IEnumerator Start()
  {
    SetState(DownloadState.CalculatingSize, true);

    yield return Downloader.StartDownloadRoutine((evt) =>
    {
      evt.SystemInitializedListener += OnInitialized;
      evt.CatalogUpdateListener += OnCatalogUpdated;
      evt.SizeDownloadedListener += OnSizeDownloaded;
      evt.DownloadProgressListener += OnDownloadProgress;
      evt.DownloadFinished += OnDownloadFinished;
      evt.ResourceListGenerateListener += OnResourceGenerated;
      evt.ResourceListGenerateFinished += OnResourceGeneratedFinished;
    });
  }

  private void SetState(DownloadState state, bool updateUI)
  {
    if (CurrentState != state) CurrentState = state;

    switch(state)
    {
      case DownloadState.CalculatingSize:
      case DownloadState.AskingDownload:
        GetObject((int)Objects.DownloadConfirm).SetActive(true);
        break;
      case DownloadState.Downloading:
      case DownloadState.ResourceGenerated:
        GetSlider((int)Sliders.DownloadSlider).gameObject.SetActive(true);
        break;
      case DownloadState.AllFinished:
        GetButton((int)Buttons.Startarea).enabled = true;
        GetButton((int)Buttons.Startarea).interactable = true;
        GetText((int)Texts.Gamestart).gameObject.SetActive(true);
        break;
      default: break;
    }

    if(updateUI)
    {
      UpdateUI();
    }
  }

  private void UpdateUI()
  {
    var descText = GetText((int)Texts.DownloadDesc);
    if (CurrentState == DownloadState.CalculatingSize)
    {
      descText.text = "다운로드 정보를 가져오고 있습니다. 잠시만 기다려주세요.";
    }
    else if(CurrentState == DownloadState.NothingToDownload)
    {
      descText.text = "이미 최신버전입니다.";
      SetState(DownloadState.ResourceGenerated, true);
    }
    else if(CurrentState == DownloadState.AskingDownload)
    {
      descText.text = $"최신 버전이 아닙니다 다운로드를 진행하시겠습니까? ({this.totalSizeUnit}{this.sizeUnit})";
    }
    else if(CurrentState == DownloadState.Downloading)
    {
      descText.text = $"{progressInfo.totalProgress}% / {progressInfo.downloadedBytes} / {progressInfo.remainBytes} / {progressInfo.totalBytes}";
      GetSlider((int)Sliders.DownloadSlider).value = progressInfo.totalProgress;
    }
    else if(CurrentState == DownloadState.DownloadFinished)
    {
      SetState(DownloadState.ResourceGenerated, true);
    }
    else if (CurrentState == DownloadState.ResourceGenerated)
    {
      if (rprogressInfo.totalCount <= 0) return;
      descText.text = $"{rprogressInfo.count} / {rprogressInfo.totalCount}";
      GetSlider((int)Sliders.DownloadSlider).value = rprogressInfo.count / rprogressInfo.totalCount;
    }
    else if(CurrentState == DownloadState.AllFinished)
    {
      GetButton((int)Buttons.Startarea).gameObject.BindEvent((evt) =>
      {
        Debug.Log("Change Scene");
        Managers.Scene.LoadScene(FScene.GameScene);
      }, FUIEvent.Click);
    }
  }

  private void OnClickStartDownload(PointerEventData evt)
  {
    SetState(DownloadState.Downloading, true);
    Downloader.GoNext();
  }

  private void OnClickCancelDownload(PointerEventData evt)
  {
#if UNITY_EDITOR
    if(Application.isEditor)
    {
      UnityEditor.EditorApplication.isPlaying = false;
    }
#else
    Application.Quit();
#endif
  }

  private void OnInitialized()
  {
    Downloader.GoNext();
  }

  private void OnCatalogUpdated()
  {
    Downloader.GoNext();
  }

  private void OnSizeDownloaded(long size)
  {
    if(size <= 0)
    {
      SetState(DownloadState.NothingToDownload, true);
      Downloader.LastValidState = DownloadController.State.NothingToDownload;
      Downloader.GoNext();
    }
    else
    {
      sizeUnit = Util.GetProperByteUnit(size);
      totalSizeUnit = Util.ConvertByteByUnit(size, sizeUnit);

      SetState(DownloadState.AskingDownload, true);
    }
  }

  private void OnDownloadProgress(DownloadProgressStatus downloadStatus)
  {
    bool changed = this.progressInfo.downloadedBytes != downloadStatus.downloadedBytes;

    progressInfo = downloadStatus;

    if(changed)
    {
      UpdateUI();

      curDownloadSizeUnit = Util.ConvertByteByUnit(downloadStatus.downloadedBytes, sizeUnit);
    }
  }

  private void OnDownloadFinished(bool isSuccess)
  {
    SetState(DownloadState.DownloadFinished, true);
    Downloader.GoNext();
  }

  private void OnResourceGenerated(ResourceProgrerssStatus resourceStatus)
  {
    rprogressInfo = resourceStatus;

    UpdateUI();
  }

  private void OnResourceGeneratedFinished()
  {
    Managers.Data.Init();

    Managers.Game.LoadGame();

    SetState(DownloadState.AllFinished, true);
    Downloader.GoNext();
  }


  //private void StartLoadAssets()
  //{
  //  Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
  //  {
  //    Debug.Log($"{key} {count}/{totalCount}");

  //    if (count == totalCount)
  //    {
  //      //데이터 초기화
  //      Managers.Data.Init();

  //      if (Managers.Game.LoadGame() == false)
  //      {
  //        Managers.Game.InitGame();
  //        Managers.Game.SaveGame();
  //      }

  //      //Managers.Scene.LoadScene(//TODO)
  //    }
  //  });
  //}
}
