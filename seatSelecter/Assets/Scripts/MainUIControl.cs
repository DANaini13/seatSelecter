﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TinyJSON;
using System.Threading;

public class MainUIControl : MonoBehaviour {
    public Text nameInput;
    public GameObject errorTextObj;
    private GameObject curErrorTextObj;
    public GameObject InputUI;
    public WebService webService;
  
    // Use this for initialization
    void Start () {
        webService = WebService.getInstance();
        webService.setSeatsStatusCallBack((Variant contant) =>
        {
            Debug.Log("slakdjflasjkdfbkjasvhfjashdfgksja");
            CheckSeat(contant);//更新座位是否为空
        });
    }
	// Update is called once per frame

    public void CheckName() 
    {
        Debug.Log(nameInput.text);
    
        webService.setName(nameInput.text, OnSetName);
    }
    private void OnSetName(Variant content)
    {
        string resultStr = (string)content["result"];
      //  Debug.Log(resultStr);
        if (resultStr == "success")
        {
       
            StartChooceSeat();
            Debug.Log("设置姓名成功");
        }
        else if (resultStr == "error-0")
        {
            Debug.Log(resultStr);
            StartChooceError();
            Debug.Log("设置姓名失败请重新输入");
        }
    }
    private void OnChooseSeat(Variant content)
    {
        Debug.Log("点击");
        string resultStr = (string)content["result"];
        Debug.Log(resultStr);
        if (resultStr == "success")
        {

            Loom.QueueOnMainThread(() => {
            //    CreatPlayer(ArrowControl.curSeatTf);
       
            });
            Debug.Log("选择成功");
        }
        else if (resultStr == "error-1")
        {
            
            Debug.Log("选择失败");
        }
    }
    private void CheckSeat(Variant contant)  //检查座位是否为空
    {

        Debug.Log("检查作为");
        Loom.QueueOnMainThread(() => {
            Transform seatTriggerTf = GameObject.Find("cubeTrigger").transform;
            for (int i = 0; i < 32; i++)
            {
                string str = (string)contant[i.ToString()];
                SeatManaer sm = seatTriggerTf.GetChild(i).GetComponent<SeatManaer>();
                Debug.Log(str);
                if (str == "NULL")
                {
                    sm.isNull = true;
                    sm.curName = "无";
                    sm.tm.text = sm.curName;
                    
                    //for (int j = 0; j < seatTriggerTf.GetChild(i).childCount; j++)
                    //{
                    //    Destroy(seatTriggerTf.GetChild(i).GetChild(j).gameObject);
                    //}
                    if (sm.curPlayerObj != null)
                    {
                        Destroy(sm.curPlayerObj);
                    }
                }
                else
                {
                    ChoiceUIControl cuc = new ChoiceUIControl();
                    if (sm.curPlayerObj == null)
                    {
                        CreatSeatPlayer(seatTriggerTf.GetChild(i), sm);
                    }
                    sm.curName = str;
                    sm.tm.text = sm.curName;
                    sm.isNull = false;
                }
            }
        });
   
    }
    public void SeatEnter() //选择作为button
    {
        webService.chooseSeat(ArrowControl.curSeatTf.GetComponent<SeatManaer>().seatNum, OnChooseSeat);
     
    }
    public void CreatPlayer(Transform tf)
    {
        if (ArrowControl.curSeatTf == null)
        {
            return;
        }
        if (ArrowControl.curSeatTf.GetComponent<SeatManaer>().isNull)
        {
            GameObject go = Instantiate((GameObject)Resources.Load("player", typeof(GameObject)), tf);
            go.transform.localPosition = new Vector3(0, 4.6f, 0);
            ArrowControl.curSeatTf.GetComponent<SeatManaer>().isNull = false;
            //     CheckRotation(go);

            switch (ArrowControl.curSeatTf.GetComponent<SeatManaer>().curVec)
            {
                case SeatManaer.seatVec.up:
                    go.transform.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case SeatManaer.seatVec.down:
                    go.transform.localEulerAngles = new Vector3(0, 180, 0);
                    break;
                case SeatManaer.seatVec.left:
                    go.transform.localEulerAngles = new Vector3(0, -90, 0);
                    break;
                case SeatManaer.seatVec.right:
                    go.transform.localEulerAngles = new Vector3(0, 90, 0);
                    break;
            }
        }
    }
    public void CreatSeatPlayer(Transform tf,SeatManaer sm)
    {
        GameObject go = Instantiate((GameObject)Resources.Load("player", typeof(GameObject)), tf);
        sm.curPlayerObj = go;
        go.transform.localPosition = new Vector3(0, 4.6f, 0);
        tf.GetComponent<SeatManaer>().isNull = false;
        //     CheckRotation(go);

        switch (tf.GetComponent<SeatManaer>().curVec)
        {
            case SeatManaer.seatVec.up:
                go.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case SeatManaer.seatVec.down:
                go.transform.localEulerAngles = new Vector3(0, 180, 0);
                break;
            case SeatManaer.seatVec.left:
                go.transform.localEulerAngles = new Vector3(0, -90, 0);
                break;
            case SeatManaer.seatVec.right:
                go.transform.localEulerAngles = new Vector3(0, 90, 0);
                break;
        }
    }
    void StartChooceSeat()
    {
       
        Loom.QueueOnMainThread(() => {
            if (curErrorTextObj != null)
            {
                DestroyImmediate(curErrorTextObj, true);
            }
            InputUI.SetActive(false);
        });
       
    }
    void StartChooceError()
    {
     
        Loom.QueueOnMainThread(() => {
            Transform canvasTf = GameObject.Find("Canvas").transform;
            if (curErrorTextObj != null)
            {
                Destroy(curErrorTextObj);
            }
            curErrorTextObj = Instantiate(errorTextObj, canvasTf);
            Debug.Log("错误");
        });
      
    }
}
