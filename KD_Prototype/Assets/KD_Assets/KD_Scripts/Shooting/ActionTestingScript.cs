using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTestingScript : MonoBehaviour {

    [SerializeField]
    GameObject instructionsPanel;

    bool hasClosedCanvas = false;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            if (hasClosedCanvas)
            {
                if (GetComponent<UnitActionScript>())
                {
                    GetComponent<UnitActionScript>().InitiateTurn();
                }
                else
                {
                    GetComponent<UnitActionMovementCounterScript>().InitiateTurn();
                }
            }
            else
            {
                instructionsPanel.SetActive(false);
                hasClosedCanvas = true;
            }
        }
	}
}
