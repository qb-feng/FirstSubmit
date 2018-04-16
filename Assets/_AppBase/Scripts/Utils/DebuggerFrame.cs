using UnityEngine;

public class DebuggerFrame : MonoBehaviour
{
    void Update()
    {
        UpdateTick();
    }

    void OnGUI()
    {
        DrawFps();
    }

    private void DrawFps()
    {
        if (s_LastFps > 50)
        {
            GUI.color = new Color(0, 1, 0);
        }
        else if (s_LastFps > 40)
        {
            GUI.color = new Color(1, 1, 0);
        }
        else
        {
            GUI.color = new Color(1.0f, 0, 0);
        }
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        GUI.Label(new Rect(50, 32, 64, 24), "fps: " + s_LastFps, style);
    }

    private long m_FrameCount = 0;
    private long m_LastFrameTime = 0;
    static long s_LastFps = 0;
    private void UpdateTick()
    {
        if (true)
        {
            m_FrameCount++;
            long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
            if (m_LastFrameTime == 0)
            {
                m_LastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
            }

            if ((nCurTime - m_LastFrameTime) >= 1000)
            {
                long fps = (long)(m_FrameCount * 1.0f / ((nCurTime - m_LastFrameTime) / 1000.0f));

                s_LastFps = fps;

                m_FrameCount = 0;

                m_LastFrameTime = nCurTime;
            }
        }
    }
    public static long TickToMilliSec(long tick)
    {
        return tick / (10 * 1000);
    }
}