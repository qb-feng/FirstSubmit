using System.Collections.Generic;
[System.Serializable]
public class ConfigTalkLibraryModel
{
    public string valuetype;
    public string id;
    public string yesvalue;
    public string novalue;
    public int SentenceAmount;
    public int AudioAmount;

    public string first;

    public string second;

    public string thirdly;

    public string fourthly;

    public static List<ConfigTalkLibraryModel> GetTalkList(List<string> ids)
    {
        var list = new List<ConfigTalkLibraryModel>();
        foreach (var id in ids)
        {
            var model = ConfigManager.Get<ConfigTalkLibraryModel>().Find(m => m.id == id);
            if (model != null)
                list.Add(model);
        }
        return list;
    }
}