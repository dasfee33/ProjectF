using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadEvent
{
  //어드레서블 시스템 초기화
  public event Action SystemInitializedListener;
  public void NotifyInitialized() => SystemInitializedListener?.Invoke();

  //Catalog 업데이트 완료
  public event Action CatalogUpdateListener;
  public void NotifyCatalogUpdate() => CatalogUpdateListener?.Invoke();

  //Size 다운로드 완료
  public event Action<long> SizeDownloadedListener;
  public void NotifySizeDownload(long size) => SizeDownloadedListener?.Invoke(size);

  //다운로드 진행
  public event Action<DownloadProgressStatus> DownloadProgressListener;
  public void NotifyDownloadProgress(DownloadProgressStatus status) => DownloadProgressListener?.Invoke(status);

  //Bundle 다운로드 완료
  public event Action<bool> DownloadFinished;
  public void NotifyDownloadFinished(bool isSuccess) => DownloadFinished?.Invoke(isSuccess);
}
