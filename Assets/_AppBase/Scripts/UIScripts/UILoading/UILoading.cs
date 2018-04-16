public class UILoading : UIBaseInit
{
    private static UILoading s_instance;
    private static bool s_force = false;

    public static void Start()
    {
        s_force = true;
        LoadingActive(true);
    }

    public static void End()
    {
        s_force = false;
        LoadingActive(false);
    }

    public static void Show()
    {
        if (!s_force)
            LoadingActive(true);
    }

    public static void Close()
    {
        if (!s_force)
            LoadingActive(false);
    }

    private static void LoadingActive(bool b)
    {
        if (s_instance == null)
        {
            var go = CreateUI("UILoading");
            s_instance = go.AddComponent<UILoading>();
        }
        s_instance.gameObject.SetActive(b);
        s_instance.transform.SetAsLastSibling();
    }
}
