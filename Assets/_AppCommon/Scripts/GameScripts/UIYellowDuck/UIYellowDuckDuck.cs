using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;

public class UIYellowDuckDuck : UIBaseInit {

    //寻路动画
    private Tween pathMove;
    private int wordIndex;
    private Vector3[] movePath;
    void Start()
    {
        wordIndex = 0;
    }

    void Update()
    {
       
    }

    public void moveDuck(Vector3[] path)
    {
        movePath = path;
        pathMove.Kill();
        float distance=0;
        for (int i = 1; i < path.Length; i++)
        {
            distance += Vector3.Distance(path[i-1], path[i]);
        }    
        pathMove=transform.DOPath(path, distance / 3f).OnWaypointChange(MyCallback);
    }
    void MyCallback(int waypointIndex)
    {
        if (waypointIndex == 0 || waypointIndex == movePath.Length)
            return;
        else
        {
            if((movePath[waypointIndex].x - movePath[waypointIndex-1].x)>=0)
            {
                UIYellowDuck.Instance.Duck.eulerAngles = new Vector3(0, 0, 0);
            }
            else 
            {
                UIYellowDuck.Instance.Duck.eulerAngles = new Vector3(0, 180, 0);
            }
        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name.StartsWith("word"))
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }
            if (UIYellowDuck.Instance.SentenceDuck.Count <= 5)
            {
                if (collider.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == UIYellowDuck.Instance.SentenceDuck[wordIndex])
                {
                    Destroy(collider.gameObject);
                    //UIYellowDuck.Instance.PanelUp.GetChild(wordIndex).GetComponent<TextMeshProUGUI>().alpha = 1;
                    UIYellowDuck.Instance.PanelUp.GetChild(wordIndex).GetComponent<TextMeshProUGUI>().DOFade(1, 2);
                    UIYellowDuck.Instance.PanelUp.GetChild(wordIndex).DOScale(1.5f, 1.5f).From();
                    wordIndex += 1;
                }
                else
                {
                    UIYellowDuck.Instance.isEnd = true;
                    pathMove.Kill();                    
                    UIYellowDuck.Instance.InitDuck();
                }
            }
            else
            {
                if (wordIndex <= 4)
                {
                    if (collider.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == UIYellowDuck.Instance.SentenceDuck[wordIndex])
                    {
                        Destroy(collider.gameObject);
                        //UIYellowDuck.Instance.PanelUp.GetChild(wordIndex).GetComponent<TextMeshProUGUI>().alpha = 1;
                        UIYellowDuck.Instance.PanelUp.GetChild(wordIndex).GetComponent<TextMeshProUGUI>().DOFade(1, 2);
                        UIYellowDuck.Instance.PanelUp.GetChild(wordIndex).DOScale(1.5f, 1.5f).From();
                        wordIndex += 1;
                    }
                    else
                    {
                        UIYellowDuck.Instance.isEnd = true;
                        pathMove.Kill();    
                       
                        UIYellowDuck.Instance.InitDuck();
                    }
                }
                else
                {
                    if (collider.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == UIYellowDuck.Instance.SentenceDuck[wordIndex])
                    {
                        Destroy(collider.gameObject);
                        //UIYellowDuck.Instance.PanelDown.GetChild(wordIndex - 5).GetComponent<TextMeshProUGUI>().alpha = 1;
                        UIYellowDuck.Instance.PanelDown.GetChild(wordIndex - 5).GetComponent<TextMeshProUGUI>().DOFade(1, 2);
                        UIYellowDuck.Instance.PanelDown.GetChild(wordIndex - 5).DOScale(1.5f, 1.5f).From();
                        wordIndex += 1;
                    }
                    else
                    {
                        UIYellowDuck.Instance.isEnd = true;
                        pathMove.Kill();    
                       
                        UIYellowDuck.Instance.InitDuck();
                    }
                }
            }
        }
        if (wordIndex == UIYellowDuck.Instance.SentenceDuck.Count)
        {
            EndDuckGame();
            UIYellowDuck.Instance.PlayAskAudioDuck();
            UIYellowDuck.Instance.RotatrText();          
            
        }
        
    }

    private void EndDuckGame()
    {
        Destroy(this.GetComponent<Rigidbody2D>());
        UIYellowDuck.Instance.isEnd = true;
       
        pathMove.Kill();
        UIYellowDuck.Instance.Astar.gameObject.GetComponent<UIYellowDuckFindPathNiko>().FindingPath(this.transform.position, UIYellowDuck.Instance.DuckTerminus);
        if (UIYellowDuck.Instance.Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path.Count != 0)
        {
            Vector3[] vecPath = new Vector3[UIYellowDuck.Instance.Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path.Count];
            for (int i = 0; i < UIYellowDuck.Instance.Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path.Count; i++)
            {
                vecPath[i] = UIYellowDuck.Instance.Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path[i]._worldPos;
            }
            UIYellowDuck.Instance.Duck.eulerAngles = new Vector3(0, 0, 0);
            moveDuck(vecPath);
        }

        pathMove.OnComplete(() =>
        {
            UITopBarStarFly fly = UIYellowDuck.Instance.FlyStar(UIYellowDuck.Instance.CurrentAsk.id, true);
            fly.OnComplete += () =>
            {
               UIYellowDuck.Instance.PlayGame();
            };
            
        });
     
    }
   
}
