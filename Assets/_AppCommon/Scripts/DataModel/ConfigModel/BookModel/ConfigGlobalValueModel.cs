[System.Serializable]
public class ConfigGlobalValueModel
{
    public int id;
    public string key;
    public string value;

    public static string GetValue(EGlobalValue key)
    {
        foreach (var item in ConfigManager.Get<ConfigGlobalValueModel>())
        {
            if (item.id == (int)key)
            {
                return item.value;
            }
        }
        return null;
    }
    public static string GetValue(string key)
    {
        foreach (var item in ConfigManager.Get<ConfigGlobalValueModel>())
        {
            if (item.key == key)
            {
                return item.value;
            }
        }
        return null;
    }
}


public enum EGlobalValue
{
    InitialPower = 1,
    PowerTime = 2,
    PowerMax = 3,
    InitialSeed = 4,
    PowerSeed = 5,
    DefaultHadFurniture = 19,
    MaxBeInvitedUserGetKey = 26,
    FlashSaleMinuteTime = 31,
    FlashSalePrice = 32,
    SevenDayStar = 33
}
