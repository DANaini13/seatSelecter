using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour {

    // Use this for initialization
    public GameObject arrowObj;
    private GameObject curArrow;
    private GameObject lastArrow;
    public static Transform curSeatTf;
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "seat")
                {
                    while (!hit.collider.GetComponent<SeatManaer>().isNull)
                    {
                        return;
                    }
                    curArrow = Instantiate(arrowObj, hit.collider.transform);
                    curArrow.transform.localPosition = new Vector3(0, 10, 0);
                    if (lastArrow != null)
                    {
                        Destroy(lastArrow);
                    }
                    lastArrow = curArrow;
                    curSeatTf = hit.collider.transform;
                    //CheckRotation(curArrow);
                    if (curSeatTf.GetComponent<SeatManaer>().curVec == SeatManaer.seatVec.up || curSeatTf.GetComponent<SeatManaer>().curVec == SeatManaer.seatVec.down)
                    {
                        curArrow.transform.localEulerAngles = new Vector3(0, 90, 0);
                    }
                }

                // Do something with the object that was hit by the raycast.
            }

        }
    }

}
