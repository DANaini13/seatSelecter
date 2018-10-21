using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceUIControl : MonoBehaviour {
    public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SeatEnter()
    {
        CreatPlayer(ArrowControl.curSeatTf);
    }
    public void CreatPlayer(Transform tf)
    {
        if (ArrowControl.curSeatTf.GetComponent<SeatManaer>().isNull)
        {
            GameObject go = Instantiate((GameObject)Resources.Load("player",typeof(GameObject)), tf);
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
    //private void CheckRotation(GameObject player)
    //{
    //    for (int i = 0; i < ArrowControl.curSeatTf.parent.childCount; i++)
    //    {
    //        if (ArrowControl.curSeatTf.parent.GetChild(i) == ArrowControl.curSeatTf)
    //        {
    //            if (i >= 16)
    //            {
    //                player.transform.localEulerAngles = new Vector3(0, 90, 0);
    //            }
    //        }
    //    }
    //}
       
}
