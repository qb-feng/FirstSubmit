using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRunningFrogRiver : UIBaseInit {

    private int randomIndex;
    private int randomIndexT;
    private Transform tempTrans;
	void Start () {
        InitCreate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void InitCreate()
    {
        //下边那一排
        randomIndex = Random.Range(6, 9);
        tempTrans = this.transform.GetChild(randomIndex);
        CreateUIItem("UIRunningFrogLotusRight", tempTrans).AddComponent<UIRunningFrogLotusRight>();
        randomIndexT = Random.Range(6, 9);
        while (randomIndexT == randomIndex)
        {
            randomIndexT = Random.Range(6, 9);
        }
        tempTrans = this.transform.GetChild(randomIndexT);
        CreateUIItem("UIRunningFrogLotusWrong", tempTrans).AddComponent<UIRunningFrogLotusWrong>();

        //中间那一排
        randomIndex = Random.Range(3, 6);
        tempTrans = this.transform.GetChild(randomIndex);
        CreateUIItem("UIRunningFrogLotusRight", tempTrans).AddComponent<UIRunningFrogLotusRight>();
        randomIndexT = Random.Range(3, 6);
        while (randomIndexT == randomIndex)
        {
            randomIndexT = Random.Range(3, 6);
        }
        tempTrans = this.transform.GetChild(randomIndexT);
        CreateUIItem("UIRunningFrogLotusRight", tempTrans).AddComponent<UIRunningFrogLotusRight>();

        //最上边那一排
        randomIndex = Random.Range(0, 3);
        tempTrans = this.transform.GetChild(randomIndex);
        CreateUIItem("UIRunningFrogLotusRight", tempTrans).AddComponent<UIRunningFrogLotusRight>();
        randomIndexT = Random.Range(0, 3);
        while (randomIndexT == randomIndex)
        {
            randomIndexT = Random.Range(0, 3);
        }
        tempTrans = this.transform.GetChild(randomIndexT);
        CreateUIItem("UIRunningFrogLotusWrong", tempTrans).AddComponent<UIRunningFrogLotusWrong>();
    }
}
