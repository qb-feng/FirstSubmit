using UnityEngine;

public class UIDesktopCarItemCar : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.Play("hit_01");
    }
}