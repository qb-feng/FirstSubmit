using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudioPlay : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// 默认播放声音
    /// </summary>
    public string m_audioClip = "ButtonClick";

    /// <summary>
    /// 按钮点击声音
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.Play(m_audioClip);
    }
}
