using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Manager : BaseInstance<Manager>
{
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        UIUtility.Tips("知道你会回来的!", TipsType.TwoButton, Quit, "Bye", "Bye");
    //    }
    //}

    //private void Quit(PointerEventData eventData)
    //{
    //    //StartCoroutine("WaitClose");
    //}

    //private IEnumerator WaitClose()
    //{
    //    AudioSource audio = AudioManager.Instance.Play("bear_bye1");
    //    yield return new WaitForSeconds(audio.clip.length);
    //    UIManager.Instance.Close();
    //    Application.Quit();
    //}
}