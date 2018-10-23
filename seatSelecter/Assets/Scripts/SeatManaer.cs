using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManaer : MonoBehaviour {
    public enum seatVec { up,down,left,right};
    public TextMesh tm;
    public seatVec curVec;
    public int seatNum;
    public bool isNull = true;
    public string curName;
    public GameObject curPlayerObj;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
