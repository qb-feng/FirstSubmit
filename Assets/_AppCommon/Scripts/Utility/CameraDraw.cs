using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraDraw : MonoBehaviour {

    private bool isPlay;
    private RawImage DrawImage;


    private WebCamTexture cameraTexture;
    private WebCamTexture frontWebcamTexture;
    private WebCamTexture rearWebcamTexture;

    private WebCamDevice[] devices;
    private WebCamTexture activeCamera;

    int width;
    int height;

    void Start()
    {
        DrawImage = GetComponent<RawImage>();
        width = (int)DrawImage.rectTransform.rect.width;
        height = (int)DrawImage.rectTransform.rect.height;
    }

    void OnDisable()
    {
        isPlay = false;
    }

    public void OpenCamera()
    {
        StartCoroutine(StartTakePhoto());
    }

    public void ChangeCamera()
    {
        if (isPlay)
        {
            activeCamera.Stop();
            if (activeCamera == frontWebcamTexture)
            {
                isFront = false;
                activeCamera = rearWebcamTexture;
            }
            else
            {
                isFront = true;
                activeCamera = frontWebcamTexture;
            }
            activeCamera.Play();
        }
        
    }
    
    public Texture2D TakePhoto()
    {
        isPlay = false;
        Texture2D texture = new Texture2D(width, height);
        if (activeCamera)
        {
            int y = 0;
            while (y < texture.height)
            {
                int x = 0;
                while (x < texture.width)
                {
                    Color color = activeCamera.GetPixel(x, y);
                    texture.SetPixel(x, y, color);
                    ++x;
                }
                ++y;
            }
            Debug.Log("-----TakePhoto-----");
            activeCamera.Stop();
        }
        texture.Apply();
        return texture;
        
    }

    private RectTransform r;
    CameraDraw cameraDraw;
    IEnumerator StartTakePhoto()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                string frontCamName = "";
                string rearCamName = "";

                for (int i = 0; i < devices.Length; i++)
                {
                    if (devices[i].isFrontFacing)
                    {
                        frontCamName = devices[i].name;
                    }
                    else
                    {
                        rearCamName = devices[i].name;
                    }
                }

                frontWebcamTexture = new WebCamTexture(frontCamName, width,height, 15);

                rearWebcamTexture = new WebCamTexture(rearCamName, width, height, 15);

                frontWebcamTexture.Stop();
                rearWebcamTexture.Stop();

                cameraDraw = this;
                Resolution screen = Screen.currentResolution;
                float f_width = screen.width;
                float f_height = screen.height;
                float f = f_width / f_height;
                r = cameraDraw.transform as RectTransform;
                r.sizeDelta = new Vector2(446 * f, 446*f);

                activeCamera = rearWebcamTexture;
                activeCamera.Play();
                isPlay = true;
            }
        }
    }

    void Update()
    {
        if (isPlay)
        {
            DrawImage.texture = activeCamera;
        }


#if UNITY_ANDROID
        if (Screen.orientation == ScreenOrientation.Landscape)
        {
            if (isFront)
            {
                DrawImage.rectTransform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                DrawImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if (Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            if (isFront)
            {
                DrawImage.rectTransform.localEulerAngles = new Vector3(0, 180, 180);
            }
            else
            {
                DrawImage.rectTransform.localEulerAngles = new Vector3(0, 0, 180);
            }
        }
#endif
    }

    public bool isFront = false;
}
