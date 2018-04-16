using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class UIFrameAnimationCtrl : MonoBehaviour
{
    private const string FramePathPrefix = "SpritesLoad/FrameAnimation/";

    [FormerlySerializedAs("m_spritePrefix")]
    public string m_folderName = "";
    public float m_frameSpeed = 1;
    public bool m_isLoop = true;

    private float m_frameIntervalTime = 1f / 7f;
    private float m_nextChangeFrameTime = 0;
    private int m_nextFrameNum = 1;
    private UnityEngine.UI.Image m_image;
    public UnityEngine.UI.Image FrameImage { get { return m_image; } }
    private Sprite m_exist;

    public delegate void FinishOnce();
    public FinishOnce OnceFinishAction;
    public FinishOnce LoopOnceFinishAction;
    private Dictionary<string, Sprite> m_frames = new Dictionary<string, Sprite>();

    // Use this for initialization
    void Awake()
    {
        m_image = GetComponent<UnityEngine.UI.Image>();
    }

    void Start()
    {
        var sprites = Resources.LoadAll<Sprite>(FramePathPrefix + m_folderName + "/Frames");
        foreach (var sprite in sprites)
        {
            m_frames.Add(sprite.name, sprite);
        }
    }

    void OnEnable()
    {
        m_nextFrameNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        FramePlay();
    }

    public void ResetBegin()
    {
        m_nextFrameNum = 1;
    }

    private void FramePlay()
    {
        m_exist = GetSprite(m_nextFrameNum);
        if (m_exist == null)
        {
            if (m_isLoop)
            {
                m_nextFrameNum = 1;

                if (LoopOnceFinishAction != null)
                    LoopOnceFinishAction();
            }
            else
            {
                enabled = false;
                if (OnceFinishAction != null)
                    OnceFinishAction();
            }
            return;
        }
        if (Time.time > m_nextChangeFrameTime)
        {
            m_nextChangeFrameTime = Time.time + m_frameIntervalTime * m_frameSpeed;
            m_image.sprite = GetSprite(m_nextFrameNum++);
        }
    }

    private Sprite GetSprite(int frame)
    {
        if (!m_frames.ContainsKey(frame.ToString()))
            return null;
        else
            return m_frames[frame.ToString()];
        //string path = m_folderName + "/" + frame;
        //return SpriteLoad.Load(path, false);
    }
}
