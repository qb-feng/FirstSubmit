using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBulletMove : MonoBehaviour
{
    /// <summary>
    ///  当前游戏对象简单的移动行为

    #region 1 - 变量

    /// <summary>
    /// 物体移动速度
    /// </summary>
    public Vector2 speed = new Vector2(100, 100);

    /// <summary>
    /// 移动方向
    /// </summary>
    public Vector2 direction = new Vector2(-2, 5);

    private Vector2 movement;

    #endregion

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 2 - 保存运动轨迹
        movement = new Vector2(speed.x * direction.x, speed.y * direction.y);

        Vector2 Bulletpos = UIManager.Instance.WorldCamera.WorldToScreenPoint(transform.position);
        if (Bulletpos.x > Screen.width || Bulletpos.y > Screen.height)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // 3 - 让游戏物体移动
        GetComponent<Rigidbody2D>().velocity = movement;
    }
    /// <summary>
    /// 检测碰撞进入
    /// </summary>
    /// <param name="other"></param>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        TextMeshProUGUI hitText = collision.transform.GetComponent<TextMeshProUGUI>();
        if (hitText)
        {
            if (hitText.text == UIShootingWords.Instance.CurrentWord.letter)
            {
                UIShootingWords.Instance.ShootingRightWord();
                Destroy(collision.transform.parent.gameObject);
            }
            else
            {
                UIShootingWords.Instance.ShootingWrongWord();               
            }
            Destroy(gameObject);
        }
    }
}
