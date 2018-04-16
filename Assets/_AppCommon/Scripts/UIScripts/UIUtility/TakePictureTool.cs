using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class TakePictureTool : UIBaseInit
{
    // Image rotation
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);

    // Image uvRect
    Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

    // Image Parent's scale
    Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    Vector3 fixedScale = new Vector3(-1f, 1f, 1f);

    private UIWebCameraTexture m_webCamTex;

    private RawImage m_cameraTexture;
    private RawImage m_picturePreview;

    // Use this for initialization
    void Start()
    {
        m_webCamTex = gameObject.AddComponent<UIWebCameraTexture>();
        m_webCamTex.OnInitedEvent = new UnityEngine.Events.UnityEvent();
        m_webCamTex.OnInitedEvent.AddListener(OnWebCamTexInited);
        m_webCamTex.requestWidth = 720;
        m_webCamTex.requestHeight = 720;
        m_webCamTex.Init();
    }

    void Destroy()
    {
        m_webCamTex.Dispose();
    }

    public void Init(RawImage cameraPicture, RawImage picturePreview)
    {
        m_cameraTexture = cameraPicture;
        m_picturePreview = picturePreview;
    }

    private void Update()
    {
        if (!m_webCamTex.isInited())
            return;
        // Rotate image to show correct orientation 
        rotationVector.z = -m_webCamTex.GetWebCamTexture().videoRotationAngle;
        m_cameraTexture.rectTransform.localEulerAngles = rotationVector;

        // Unflip if vertically flipped
        m_cameraTexture.uvRect = m_webCamTex.GetWebCamTexture().videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        m_cameraTexture.transform.parent.localScale = m_webCamTex.GetWebCamDevice().isFrontFacing ? fixedScale : defaultScale;
    }

    private void OnWebCamTexInited()
    {
        var tex = m_webCamTex.GetWebCamTexture();
        m_cameraTexture.texture = tex;
        m_cameraTexture.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
    }

    public void CameraPlay()
    {
        m_webCamTex.Play();
    }

    public void CameraPause()
    {
        m_webCamTex.Pause();
    }

    public void CameraStop()
    {
        m_webCamTex.Stop();
    }

    /// <summary>
    /// 拍照
    /// </summary>
    public void TakePicture()
    {
        StartCoroutine(GetTexture());
    }

    /// <summary>
    /// 切换摄像头
    /// </summary>
    public void ChangeCamera()
    {
        if (m_webCamTex.isInited())
        {
            var tex = m_webCamTex.GetWebCamTexture();
            m_webCamTex.Init(null, tex.width, tex.height, !m_webCamTex.requestIsFrontFacing);
        }
    }

    public IEnumerator GetTexture()
    {
        var webCamTex = m_webCamTex.GetWebCamTexture();
        webCamTex.Pause();
        yield return new WaitForEndOfFrame();

        // 确定预览图，要截取镜头的大小
        int w = webCamTex.width;
        w = webCamTex.height > w ? w : webCamTex.height;
        Vector2 offset = new Vector2((webCamTex.width - w) / 2, (webCamTex.height - w) / 2);

        Texture2D preivew = null;
        var webCamDevice = m_webCamTex.GetWebCamDevice();
        if (webCamDevice.isFrontFacing)
        {
#if UNITY_ANDROID
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                preivew = HorizontalFlipPic(webCamTex, new Vector2(w, w), offset);
            }
            else
            {
                preivew = VerticalFlipPic(webCamTex, new Vector2(w, w), offset);
            }
#elif UNITY_IOS
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                preivew = VerticalFlipPic(webCamTex, new Vector2(w, w), offset);
            }
            else
            {
                preivew = HorizontalFlipPic(webCamTex, new Vector2(w, w), offset);
            }
#endif
        }
        else
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                var tex = new Texture2D(w, w, TextureFormat.ARGB32, false);
                var colors = webCamTex.GetPixels((int)offset.x, (int)offset.y, w, w);
                tex.SetPixels(colors);
                tex.Apply();
                preivew = tex;
            }
            else
            {
                preivew = VerticalFlipPic(webCamTex, new Vector2(w, w), offset);
                preivew = HorizontalFlipPic(preivew, new Vector2(w, w), Vector2.zero);
            }
        }

        m_picturePreview.texture = preivew;
    }

    /// <summary>
    /// 水平翻转
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="size"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Texture2D HorizontalFlipPic<T>(T tex, Vector2 size, Vector2 offset) where T : Texture
    {
        int width = (int)size.x;//新图片的宽度
        int height = (int)size.y;//新图片的高度
        int offsetX = (int)offset.x;
        int offsetY = (int)offset.y;

        var newTex = new Texture2D(width, height);//创建一张同等大小的空白图片 

        int i = 0;

        while (i < width)
        {
            Color[] singleLine = null;
            if (tex is WebCamTexture)
            {
                var webCamTex = tex as WebCamTexture;
                singleLine = webCamTex.GetPixels(offsetX + i, offsetY, 1, height);
            }
            else if (tex is Texture2D)
            {
                var tex2d = tex as Texture2D;
                singleLine = tex2d.GetPixels(offsetX + i, offsetY, 1, height);
            }
            newTex.SetPixels(width - i - 1, 0, 1, height, singleLine);
            i++;
        }
        newTex.Apply();

        return newTex;
    }

    /// <summary>
    /// 竖直翻转
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="size"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Texture2D VerticalFlipPic<T>(T tex, Vector2 size, Vector2 offset) where T : Texture
    {
        int width = (int)size.x;//新图片的宽度
        int height = (int)size.y;//新图片的高度
        int offsetX = (int)offset.x;
        int offsetY = (int)offset.y;

        var newTex = new Texture2D(width, height);//创建一张同等大小的空白图片 

        int i = 0;

        while (i < height)
        {
            Color[] singleLine = null;
            if (tex is WebCamTexture)
            {
                var webCamTex = tex as WebCamTexture;
                singleLine = webCamTex.GetPixels(offsetX, offsetY + i, width, 1);
            }
            else if (tex is Texture2D)
            {
                var tex2d = tex as Texture2D;
                singleLine = tex2d.GetPixels(offsetX, offsetY + i, width, 1);
            }
            newTex.SetPixels(0, height - i - 1, width, 1, singleLine);
            i++;
        }
        newTex.Apply();

        return newTex;
    }
}
