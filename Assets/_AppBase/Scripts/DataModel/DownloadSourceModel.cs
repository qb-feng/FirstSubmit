using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SourceDownloadState
{
    None,
    Download,
    Update,
    Waiting,
    Downloading,
    Complete,
}

public class DownloadSourceModel
{
    public DownloadPriority priority;
    public SourceDownloadState state;
    public System.Action onComplete;
    public float progress;
    public Dictionary<string, SourceDownloadState> download;
}
