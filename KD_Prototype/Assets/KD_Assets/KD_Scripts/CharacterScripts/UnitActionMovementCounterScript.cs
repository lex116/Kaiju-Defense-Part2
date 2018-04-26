using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionMovementCounterScript : MonoBehaviour {

    [SerializeField]
    Text distanceTraveledText;
    [SerializeField]
    float maxDistance = 10f;

    CustomCharacterController charController;
    bool isCountingMovement;
    Vector3 previousPosition;
    float distanceTraveled;
    private void Start()
    {
        charController = GetComponent<CustomCharacterController>();
        charController.enabled = false;
    }

    public void InitiateTurn()
    {
        previousPosition = transform.position;
        distanceTraveled = 0f;
        charController.enabled = true;
    }

    private void FixedUpdate()
    {
        if (previousPosition != transform.position)
        {
            distanceTraveled += Mathf.Abs((previousPosition - transform.position).magnitude);
            previousPosition = transform.position;
            distanceTraveledText.text = "" + (int)distanceTraveled;
        }
        if (distanceTraveled >= maxDistance)
        {
            charController.enabled = false;
        }
    }
}
