using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UICupidBallonItem : UIBaseInitItem
{
    private UICupid Model;
    private TextMeshProUGUI TextWord { get { return Get<TextMeshProUGUI>("TextWord"); } }
    public void Init(string word,UICupid model)
    {
        GetComponent<Image>().sprite = model.GetS("UICupidBalloon" + Random.Range(1, 7));

        Model = model;

        TextWord.text = word;

        GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-620, 620), 0);

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 3);

        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "UICupidArrow(Clone)")
        {
            Model.Check(TextWord.text);
            AudioManager.Instance.Play("BallonDestroy");
            Destroy(gameObject);
        }
    }
}