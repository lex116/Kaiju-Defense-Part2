using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSightScript : MonoBehaviour
{
    LineRenderer lineRenderer;

    public bool HasTarget = false;

	// Use this for initialization
	void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();	
	}
	
	// Update is called once per frame
	//void Update ()
 //   {
 //       if(HasTarget)
 //       {
 //           RaycastHit hit;

 //           if (Physics.Raycast(transform.position, transform.forward, out hit))
 //           {
 //               if (hit.collider)
 //               {
 //                   lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));
 //               }

 //               else
 //               {
 //                   lineRenderer.SetPosition(1, new Vector3(0, 0, 5000));
 //               }
 //           }
 //       }

 //       else
 //       {
 //           lineRenderer.SetPosition(1, new Vector3(0, 0, 0));
 //       }
	//}
}
