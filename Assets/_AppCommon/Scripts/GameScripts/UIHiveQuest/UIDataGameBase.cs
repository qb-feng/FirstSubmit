using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// 游戏模型的继承类 - 用来获取所有的混淆数据的二次基类
/// </summary>
public class UIDataGameBase : UIBaseLevelGame
{
    //共有变量
    protected bool IsHaveAuioPlayer = true;//是否拥有播放器 - 默认有，子类没有的话需要在Refresh之前定义为false
    protected GameObject AudioPlayer { get { return Get("AudioPlayer"); } }//播放器
    protected AudioSource asource;
    protected TextMeshProUGUI ShowText { get { return Get<TextMeshProUGUI>("ShowText"); } }//文字显示器
    protected Image ShowImage { get { return Get<Image>("ShowImage"); } }//图片显示器
    protected bool isGameEnd = false;//游戏是否结束

    /// <summary>
    /// 子类继承必须实现该父类方法
    /// </summary>
    public override void Refresh()
    {
        base.Refresh();
        if (IsHaveAuioPlayer)
        {
            //添加语音播放事件
            UGUIEventListener.Get(AudioPlayer).onPointerClick += (click) => { PlayAudio(); };
        }
    }
    protected virtual void RefreshData() { }//数据重置
    public override void PlayGame()
    {
        RefreshData();

       // Debug.Log(ResourceManager.Instance.("CommonPictureRope"));
    }
    /// <summary>
    /// 虚方法 - 在游戏结束后执行 - 供子类重写，在里面做一些游戏结束后资源释放的操作
    /// </summary>
    protected virtual void OnGameEnd()
    {
        isGameEnd = true;
    }


    /// <summary>
    /// 播放当前语音的方法 - 可重写
    /// 参数1 ：是否强制开启，默认为不强制开启
    /// </summary>
    protected virtual void PlayAudio(bool forceOpen = false)
    {
        if (asource != null && asource.isPlaying)
        {
            if (forceOpen)
            {
                //强制开启模式时 - 直接关闭当前播放音，开启下一个播放音
                asource.Stop();
            }
            else
            {
                //普通开启模式时 - 直接退出
                return;
            }
        }
        switch (StartData.dataType)
        {
            case DataType.Word:
                asource = CurrentWord.PlaySound();
                break;
            case DataType.Say:
                asource = CurrentSay.PlayAnswerSound();
                break;
            case DataType.Ask:
                CurrentAsk.PlaySentenceSound();//问句的话默认是播放整个句子的完整读音
                break;
        }
    }


    protected List<T> GetFalseDataList<T>()
    {
        return new List<T>();
    }

    /// <summary>
    /// 返回指定单词的混淆项数据 （剔除过当前单词，同时返回的容器也是随机过的）
    /// </summary>
    protected List<ConfigWordLibraryModel> GetFalseConfigWordRandomList(ConfigWordLibraryModel currentWord)
    {
        List<ConfigWordLibraryModel> temp = new List<ConfigWordLibraryModel>();
        string currentWordId = currentWord.id.Substring(0, 5);//找到指定单词的同单元单词id
        foreach (var v in ConfigManager.Get<ConfigWordLibraryModel>())
        {
            if (v.id.Substring(0, 5).Equals(currentWordId))
                temp.Add(v);
        }
        //剔除当前单词
        temp.Remove(currentWord);
        //打乱该容器
        RandomSortList(temp);
        //返回该容器
        return temp;
    }

    /// <summary>
    /// 得到当前类型的单词（单词返回该单词的每一个字母(不去空格！！保留空格)）或者句子（已经切割好的） - 返回List数组
    /// 参数1:单词模式时是否需要处理空格字母
    /// </summary>
    /// <returns></returns>
    protected List<string> GetCurrntTextListString(bool wordType_DealSpace = false)
    {
        List<string> currentStirng = new List<string>();
        switch (StartData.dataType)
        {
            case DataType.Word:
                if (wordType_DealSpace)
                {                 
                    //处理单词中的空格字符串 - 将该空格放到空格的前一个字符中
                    for (int i = 0; i < CurrentWord.word.Length; ++i)
                    {
                        string temp = CurrentWord.word[i].ToString();
                        if (temp.Equals(" ") && i > 0)
                        {
                            currentStirng[i - 1] = currentStirng[i - 1] + " ";
                            continue;
                        }
                        currentStirng.Add(temp);
                    }
                }
                else 
                {
                    //不处理 - 空格照样返回                 
                    foreach (var v in CurrentWord.word.ToCharArray(0,CurrentWord.word.Length))
                    {
                        currentStirng.Add(v.ToString());
                    }
                }
                break;

            case DataType.Say:
                string[] says = CurrentSay.yes.Split('_');
                foreach (var v in says)
                    currentStirng.Add(v);
                break;

            case DataType.Ask:
                string[] asks = (CurrentAsk.yes + "_" + CurrentAsk.answer).Split('_');
                foreach (var v in asks)
                    currentStirng.Add(v);
                break;
        }
        return currentStirng;
    }

    /// <summary>
    /// 获取当前图集
    /// </summary>
    /// <returns></returns>
    protected Sprite GetCurrentSprite() 
    {
        Sprite currenSprite = null;
        switch (StartData.dataType) 
        {
            case DataType.Say:
                currenSprite = CurrentSay.sprite;
                break;
            case DataType.Word:
                currenSprite = CurrentWord.sprite;
                break;
            case DataType.Ask:
                currenSprite = CurrentAsk.sprite;
                break;            
        }
        return currenSprite;
    }
    /// <summary>
    /// 随机打乱一个数组
    /// </summary>
    protected void RandomSortList<T>(List<T> list)
    {
        List<int> randomSeries = new List<int>();//随机序列 - 用来随机间单词打乱的一个序列
        GetRandomSeries(randomSeries, 0, list.Count);
        List<T> temp = new List<T>();
        for (int i = 0; i < list.Count; ++i)
        {
            temp.Add(list[randomSeries[i]]);
        }
        list = temp;
        temp = null;
    }

    /// <summary>
    /// 得到一个随机序列保存在RandomSeries数组中：如1 - 7的随机序列（不包括7）
    /// </summary>
    protected void GetRandomSeries(List<int> randomSeries, int startIndex, int endIndex)
    {
        if (startIndex >= endIndex)
            return;

        int result = Random.Range(startIndex, endIndex);
        randomSeries.Add(result);

        GetRandomSeries(randomSeries, startIndex, result);
        GetRandomSeries(randomSeries, result + 1, endIndex);
    }
}
