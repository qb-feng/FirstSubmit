using System;
using System.Collections.Generic;
using System.IO;
using Tangzx.ABSystem;
using UnityEngine;

public enum DownloadPriority
{
    Lower,
    Low,
    Normal,
    High,
    Higher,
}

public class UpdateAssetBundleManager : BaseInstance<UpdateAssetBundleManager>
{
    public string ABDownloadUrl
    {
        get
        {
            //return AssetBundleManager.Instance.pathResolver.GetBundleSourceFile("");
            return "";//ConfigVersionModel.GetVersion().ABDownloadUrl;
        }
    }

    private string DepAllFilePath { get { return Path.Combine(BundleCacheDir, "dep.all"); } }

    private string CacheAllFilePath { get { return Path.Combine(BundleCacheDir, "cache.all"); } }

    private string DepAllVersionPath { get { return Path.Combine(BundleCacheDir, "version"); } }

    private string BundleCacheDir { get { return AssetBundleManager.Instance.pathResolver.BundleCacheDir; } }

    private string m_abVersion;

    /// <summary>
    /// 所有资源列表
    /// </summary>
    private AssetBundleDataReader m_depAll;

    public AssetBundleDataReader DepAll { get { return m_depAll; } }

    /// <summary>
    /// StreamingAssets下的所有资源
    /// </summary>
    private AssetBundleDataReader m_streamingAll;

    /// <summary>
    /// 已经下载缓存的资源列表
    /// </summary>
    private Dictionary<string, AssetBundleData> m_cacheAll;

    /// <summary>
    /// 需要更新资源的列表
    /// </summary>
    private Dictionary<string, AssetBundleData> m_updateAll = new Dictionary<string, AssetBundleData>();

    private List<DownloadSourceModel> m_downloadQueue = new List<DownloadSourceModel>();

    void Awake()
    {
        ReaderCacheAll();
    }

#if TEACHER_BRANCH || STUDENT_BRANCH
	void Start()
	{
		Player.Instance.OnUserInitEvent += OnUserInitEvent;
	}

	void OnDestroy()
	{
		Player.Instance.OnUserInitEvent -= OnUserInitEvent;
	}

	private void OnUserInitEvent()
	{
		if(m_lessonSource != null)
			m_lessonSource.Clear();
		m_updateAll.Clear();
		m_downloadQueue.Clear();
	}
#endif

    private void Update()
    {
        UpdateDownloadSourceQueue(m_downloadQueue);
    }

    private void UpdateDownloadSourceQueue(List<DownloadSourceModel> downloadSourceQueue)
    {
        if (downloadSourceQueue.Count > 0)
        {
            var model = downloadSourceQueue[0];
            if (model.state == SourceDownloadState.Complete)
            {
                downloadSourceQueue.RemoveAt(0);
                if (downloadSourceQueue.Count > 0)
                    model = downloadSourceQueue[0];
                else
                    return;
            }
            if (model.state == SourceDownloadState.Waiting)
            {
                model.state = SourceDownloadState.Downloading;
            }
            if (model.state == SourceDownloadState.Downloading)
            {
                int completeNum = 0;
                var existList = new List<string>();
                foreach (var item in model.download)
                {
                    if (item.Value == SourceDownloadState.Downloading)
                        break;
                    else if (item.Value == SourceDownloadState.Download || item.Value == SourceDownloadState.Update)
                    {
                        if (m_cacheAll.ContainsKey(item.Key))
                        {
                            existList.Add(item.Key);
                            completeNum++;
                        }
                        else
                        {
                            var request = ServerManager.GetRequest<DownloadFileRequeset>(gameObject);
                            if (request.IsRequesting)
                                break;
                            model.download[item.Key] = SourceDownloadState.Downloading;
                            string savePath = Path.Combine(BundleCacheDir, item.Key);
                            string downloadUrl = Utility.CombineUrl(UpdateAssetBundleManager.Instance.ABDownloadUrl, Path.GetFileName(savePath));
                            request.Send(savePath, downloadUrl);
                            request.OnSuccEvent += path =>
                            {
                                string fileName = Path.GetFileName(path);
                                model.download[fileName] = SourceDownloadState.Complete;
                                if (m_depAll.infoMap.ContainsKey(Path.GetFileName(path)))
                                    WriterCacheAll(m_depAll.infoMap[Path.GetFileName(path)]);
                            };
                            request.OnErrorEvent += path =>
                            {
                                string fileName = Path.GetFileName(path);
                                if (m_updateAll.ContainsKey(fileName))
                                    model.download[fileName] = SourceDownloadState.Update;
                                else
                                    model.download[fileName] = SourceDownloadState.Download;
                            };
                            break;
                        }
                    }
                    else if (item.Value == SourceDownloadState.Complete)
                        completeNum++;
                }
                foreach (var item in existList)
                {
                    model.download[item] = SourceDownloadState.Complete;
                }
                if (model.download.Count == completeNum)
                {
                    model.state = SourceDownloadState.Complete;
                }
                model.progress = completeNum / (float)model.download.Count;
            }
        }
    }

    public void InitDepFile(bool update, Action action)
    {
        bool exist = File.Exists(DepAllFilePath);
        if (!exist)
            update = true;
        string localABVersion = ConfigVersionModel.GetVersion().abVersion;
        if (localABVersion.CompareTo(m_abVersion) > 0)
            update = true;
        if (!update)
        {
            InitDepFileHelper(action);
        }
        else
        {
            var request = ServerManager.GetRequest<DownloadFileRequeset>(gameObject);
            string downloadUrl = Utility.CombineUrl(UpdateAssetBundleManager.Instance.ABDownloadUrl, Path.GetFileName(DepAllFilePath));
            request.Send(DepAllFilePath, downloadUrl);
            request.OnSuccEvent += path =>
            {
                InitDepFileHelper(action);
                using (var sw = new StreamWriter(DepAllVersionPath))
                {
                    m_abVersion = ConfigVersionModel.GetVersion().abVersion;
                    sw.WriteLine(m_abVersion);
                }
            };
        }
    }

    public void InitDepFileHelper(Action action)
    {
        ReaderStreamingAll();
        ReaderDepAll();
        FilterNeedUpdate();
        if (action != null)
            action();
    }

    private void FilterNeedUpdate()
    {
        foreach (var cache in m_cacheAll)
        {
            if (m_depAll.infoMap.ContainsKey(cache.Key))
            {
                var dep = m_depAll.infoMap[cache.Key];
                if (!dep.hash.Equals(cache.Value.hash))
                    m_updateAll.Add(dep.fullName, dep);
            }
        }
    }

    private void ReaderDepAll()
    {
        if (File.Exists(DepAllFilePath))
        {
            using (var fs = new FileStream(DepAllFilePath, FileMode.Open))
            {
                m_depAll = new AssetBundleDataReader();
                m_depAll.Read(fs);
                fs.Close();
            }
        }
        else
        {
            m_depAll = m_streamingAll;
        }
    }

    private void ReaderStreamingAll()
    {
        m_streamingAll = AssetBundleManager.Instance.depInfoReader;
    }

    private void ReaderCacheAll()
    {
        using (var fs = new FileStream(CacheAllFilePath, FileMode.OpenOrCreate))
        {
            var reader = new AssetBundleDataReader();
            reader.Read(fs);
            m_cacheAll = reader.infoMap;
            fs.Close();
        }
        var files = Directory.GetFiles(BundleCacheDir, "*.ab");
        var existFiles = new Dictionary<string, AssetBundleData>();
        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            if (m_cacheAll.ContainsKey(fileName))
                existFiles.Add(fileName, m_cacheAll[fileName]);
        }
        var writer = new AssetBundleDataWriter();
        writer.Save(CacheAllFilePath, new List<AssetBundleData>(existFiles.Values));
        m_cacheAll = existFiles;

        if (File.Exists(DepAllVersionPath))
        {
            using (var sr = new StreamReader(DepAllVersionPath))
            {
                m_abVersion = sr.ReadLine();
            }
        }
    }

    private void WriterCacheAll(AssetBundleData data)
    {
        if (m_cacheAll.ContainsKey(data.fullName))
            m_cacheAll[data.fullName] = data;
        else
            m_cacheAll.Add(data.fullName, data);
        var writer = new AssetBundleDataWriter();
        writer.Save(CacheAllFilePath, new List<AssetBundleData>(m_cacheAll.Values));

        if (m_updateAll.ContainsKey(data.fullName))
            m_updateAll.Remove(data.fullName);
    }

    private T FillDownloadSourceData<T>(T model, List<string> sourceList) where T : DownloadSourceModel
    {
        model.state = SourceDownloadState.None;
        model.download = new Dictionary<string, SourceDownloadState>();
        bool isExistDownload = false;
        bool isExistUpdate = false;
        foreach (var shortName in sourceList)
        {
            if (m_streamingAll.GetAssetBundleInfoByShortName(shortName.ToLower()) != null)
                continue;
            var abInfo = m_depAll.GetAssetBundleInfoByShortName(shortName.ToLower());
            if (abInfo == null)
                continue;
            var allABName = new Dictionary<string, string>();
            GetAllABName(allABName, abInfo);
            foreach (var abName in allABName.Keys)
            {
                if (m_cacheAll.ContainsKey(abName))
                {
                    if (m_updateAll.ContainsKey(abName))
                    {
                        if (!model.download.ContainsKey(abName))
                            model.download.Add(abName, SourceDownloadState.Update);
                        isExistUpdate = true;
                    }
                }
                else
                {
                    if (!model.download.ContainsKey(abName))
                        model.download.Add(abName, SourceDownloadState.Download);
                    isExistDownload = true;
                }
            }
        }
        if (isExistDownload)
        {
            model.state = SourceDownloadState.Download;
        }
        else if (isExistUpdate)
            model.state = SourceDownloadState.Update;
        else if (sourceList.Count > 0 && model.download.Count == 0)
            model.state = SourceDownloadState.Complete;
        return model;
    }

    private void GetAllABName(Dictionary<string, string> depInfo, AssetBundleData abd)
    {
        if (!depInfo.ContainsKey(abd.fullName))
            depInfo.Add(abd.fullName, abd.fullName);
        foreach (var abName in abd.dependencies)
        {
            if (!depInfo.ContainsKey(abName))
                depInfo.Add(abName, abName);
            GetAllABName(depInfo, m_depAll.infoMap[abName]);
        }
    }

    public void BeginDownloadSource<T>(T model) where T : DownloadSourceModel
    {
        if (!m_downloadQueue.Contains(model) && model.state != SourceDownloadState.Complete)
        {
            foreach (var abName in new List<string>(model.download.Keys))
            {
                if (m_cacheAll.ContainsKey(abName))
                {
                    model.download.Remove(abName);
                }
            }
            model.state = SourceDownloadState.Waiting;
            m_downloadQueue.Add(model);
            m_downloadQueue.Sort((x, y) =>
            {
                if (x.state == SourceDownloadState.Downloading)
                {
                    return -1;
                }
                return -(x.priority).CompareTo(y.priority);
            });
        }
    }

    DownloadSourceModel m_wordRecord;
    public DownloadSourceModel GetGameSourceDownloadState(int gameId)
    {
        List<string> sourceList = new List<string>();
        m_wordRecord = new DownloadSourceModel();
        m_wordRecord.priority = DownloadPriority.High;

        foreach (var v in ConfigManager.Get<ConfigGameSourceModel>())
        {
            if (v.gameId == gameId)
            {
                sourceList.Add(v.source + v.extension);
            }
        }

        FillDownloadSourceData(m_wordRecord, sourceList);

        return m_wordRecord;
    }
}