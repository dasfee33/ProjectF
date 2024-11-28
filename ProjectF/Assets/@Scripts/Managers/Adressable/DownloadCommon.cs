using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DownloadProgressStatus
{
  public long downloadedBytes;
  public long totalBytes;
  public long remainBytes;
  public float totalProgress; // 0 ~ 1

  public DownloadProgressStatus(long downloadedBytes, long totalBytes, long remainBytes, float totalProgress)
  {
    this.downloadedBytes = downloadedBytes;
    this.totalBytes = totalBytes;
    this.remainBytes = remainBytes;
    this.totalProgress = totalProgress;
  }

}
