using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionScript : MonoBehaviour {
    [SerializeField]
    Light moveRangeLightPrefab;
    [SerializeField]
    Light runRangeLightPrefab;
    [SerializeField]
    float movementRange = 10f;
    [SerializeField]
    float runRange = 20f;
    [SerializeField]
    float lightHeightDistance = 10f;

    Vector3 startingPosition;


    public void InitiateTurn()
    {
        //Activate player controller
        GetComponent<CustomCharacterController>().enabled = true;

        //Calculate light pos based on player position and light height distance
        Vector3 lightPosition = new Vector3(transform.position.x, transform.position.y + lightHeightDistance, transform.position.z);
        //Set rotation of lights to point at ground
        Quaternion lightRotation = new Quaternion();
        lightRotation.eulerAngles = new Vector3(90,0,0);
        //Instantiate indicator lights
        Light moveRangeLight = Instantiate(moveRangeLightPrefab, lightPosition, lightRotation);
        Light runRangeLight = Instantiate(runRangeLightPrefab, lightPosition, lightRotation);

        //Set indicator lights range and angles
            //Add 1 unit to range because Unity likes to fully cross the projection plane
        moveRangeLight.spotAngle = Mathf.Atan(movementRange * 4f / lightPosition.y) * Mathf.Rad2Deg;
        Debug.Log("Move range light spot angle = " + moveRangeLight.spotAngle);
        moveRangeLight.range = lightPosition.y + 10f;

        runRangeLight.spotAngle = moveRangeLight.spotAngle * 4f;
        Debug.Log("Run range light spot angle = " + runRangeLight.spotAngle);
        runRangeLight.range = lightPosition.y + 10f;

    }
}
