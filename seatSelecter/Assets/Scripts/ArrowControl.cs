using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour {

    // Use this for initialization
    public GameObject arrowObj;
    private GameObject curArrow;
    private GameObject lastArrow;
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
                    curArrow =  Instantiate(arrowObj, hit.collider.transform);
                    curArrow.transform.localPosition = new Vector3(0, 10, 0);
                    if (lastArrow != null)
                    {
                        Destroy(lastArrow);
                    }
                    lastArrow = curArrow;
                }

                // Do something with the object that was hit by the raycast.
            }

        }
    }
}
