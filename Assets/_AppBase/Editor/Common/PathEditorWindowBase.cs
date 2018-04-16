using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public class PathEditorWindowBase : EditorWindow
{
    public static string GetConfigPath(string configName)
    {
        var scriptGUID = AssetDatabase.FindAssets(configName);
        foreach (var item in scriptGUID)
        {
            string configPath = AssetDatabase.GUIDToAssetPath(item);
            if (Path.GetFileNameWithoutExtension(configPath).Equals(configName))
            {
                string dirPath = System.IO.Path.GetDirectoryName(configPath);
                return string.Format("{0}/{1}.asset", dirPath, configName);
            }
        }
        return "";
    }

    protected static string SaveConfigPath { get; set; }

    private static PathConfigScriptable m_pathConfig;
    protected static PathConfigScriptable PathConfig
    {
        get
        {
            if (m_pathConfig == null)
            {
                if (SaveConfigPath != null)
                    m_pathConfig = AssetDatabase.LoadAssetAtPath<PathConfigScriptable>(SaveConfigPath);
                if (m_pathConfig == null)
                {
                    m_pathConfig = ScriptableObject.CreateInstance<PathConfigScriptable>();
                }
            }
            return m_pathConfig;
        }
        set { m_pathConfig = value; }
    }
    private ReorderableList _list;
    private Vector2 _scrollPosition = Vector2.zero;

    void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
    {
        const float GAP = 5;

        var model = PathConfig.configList[index];
        rect.y++;

        Rect r = rect;

        r.xMin = 20;
        r.xMax = rect.xMax / 2 - 50;
        GUI.enabled = false;
        model.loadPath = GUI.TextField(r, model.loadPath);
        GUI.enabled = true;

        r.xMin = r.xMax + GAP;
        r.width = 50;
        if (GUI.Button(r, "Select"))
        {
            model.loadPath = SelectFolder();
        }

        r.xMin = r.xMax + GAP;
        r.xMax = rect.xMax - 50;
        GUI.enabled = false;
        model.savePath = GUI.TextField(r, model.savePath);
        GUI.enabled = true;

        r.xMin = r.xMax + GAP;
        r.width = 50;
        if (GUI.Button(r, "Select"))
        {
            model.savePath = SelectFolder();
        }
    }

    string SelectFolder()
    {
        string dataPath = Application.dataPath;
        string selectedPath = EditorUtility.OpenFolderPanel("选择文件夹", dataPath, "");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (selectedPath.StartsWith(dataPath))
            {
                return "Assets/" + selectedPath.Substring(dataPath.Length + 1);
            }
            else
            {
                ShowNotification(new GUIContent("不能在Assets目录之外!"));
            }
        }
        return "";
    }

    void OnListHeaderGUI(Rect rect)
    {
        EditorGUI.LabelField(rect, "路径列表");
    }

    void InitFilterListDrawer()
    {
        _list = new ReorderableList(PathConfig.configList, typeof(PathConfigScriptable));
        _list.drawElementCallback = OnListElementGUI;
        _list.drawHeaderCallback = OnListHeaderGUI;
        _list.draggable = true;
        _list.elementHeight = 22;
        _list.onAddCallback = (list) => Add();
    }

    void Add()
    {
        var model = new PathConfigScriptable.ConvertModel();
        model.loadPath = "";
        model.savePath = "";
        PathConfig.configList.Add(model);
    }

    void OnGUI()
    {
        if (_list == null)
        {
            InitFilterListDrawer();
        }

        bool execBuild = false;
        //tool bar
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
            {
                Save();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Build", EditorStyles.toolbarButton))
            {
                execBuild = true;
            }
        }
        GUILayout.EndHorizontal();

        //context
        GUILayout.BeginVertical();
        {
            GUILayout.Space(10);

            //Filter item list
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            {
                _list.DoLayoutList();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

        //set dirty
        if (GUI.changed)
            EditorUtility.SetDirty(PathConfig);

        if (execBuild)
            Build();
    }

    void Save()
    {
        if (AssetDatabase.LoadAssetAtPath<PathConfigScriptable>(SaveConfigPath) == null)
        {
            AssetDatabase.CreateAsset(PathConfig, SaveConfigPath);
        }
        else
        {
            EditorUtility.SetDirty(PathConfig);
        }
    }

    protected virtual void Build() { }
}
