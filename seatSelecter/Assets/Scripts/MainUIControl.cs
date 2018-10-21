using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TinyJSON;

public class MainUIControl : MonoBehaviour {
    public Text nameInput;
    public GameObject errorTextObj;
    public GameObject InputUI;
    public WebService webService;
    // Use this for initialization
    void Start () {
        webService = WebService.getInstance();
        webService.setSeatsStatusCallBack((Variant contant) =>
        {
            CheckSeat(contant);
        });

    }
	
	// Update is called once per frame

    public void CheckName()
    {
        webService.setName(nameInput.text, OnSetName);
    }
    private void OnSetName(Variant content)
    {
        string resultStr = (string)content["result"];
        if (resultStr == "success")
        {
            InputUI.SetActive(false);
            Debug.Log("设置姓名成功");
        }
        else if (resultStr == "error reason")
        {
            Transform canvasTf = GameObject.Find("Canvas").transform;
            GameObject errorObj = Instantiate(errorTextObj, canvasTf);
            Debug.Log("设置姓名失败请重新输入");
        }
    }
    private void CheckSeat(Variant contant)  //检查座位是否为空
    {
        Transform seatTriggerTf = GameObject.Find("cubeTrigger").transform;
        SeatManaer sm = seatTriggerTf.GetComponent<SeatManaer>();
        for (int i = 0; i < 32; i++)
        {
            string str = (string)contant[i.ToString()];
            if (str == "Null")
            {
                sm.curName = "Null";
                sm.isNull = true;
                for (int j = 0; j < seatTriggerTf.childCount; j++)
                {
                    Destroy(seatTriggerTf.GetChild(j).gameObject);
                }
            }
            else
            {
                ChoiceUIControl cuc = new ChoiceUIControl();
                cuc.CreatPlayer(seatTriggerTf.GetChild(i));
            }
        }
    }


}
