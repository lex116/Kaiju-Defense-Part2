using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KD_CharacterController_Tank : KD_CharacterController
{
    public GameObject Turret;
    public GameObject Barrel;
    public GameObject Body;
    internal int negativeBodyRotationModifier = 10;

    internal int TankxRotMaxUp = -45;
    internal int TankxRotMinDown = 5;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();
        xRotMaxUp = TankxRotMaxUp;
        xRotMinDown = TankxRotMinDown;
    }

    #region Methods

    // Use this for every frame jolly good tip tip
    public override void InputUpdate()
    {
        if (!cantMove)
        {
            RotateBody();
            ThrottleBody();
        }

        if (!cantLook)
        {
            RotateTurret();
        }

        GroundCheck();
    }

    public void RotateBody()
    {
        float horizontal = Input.GetAxis("Horizontal");

        float rotAmountY = horizontal * walkSpeed / negativeBodyRotationModifier;

        Vector3 targetRotBody = Body.transform.rotation.eulerAngles;

        targetRotBody.x = 0;
        targetRotBody.z = 0;
        targetRotBody.y += rotAmountY;

        Body.transform.rotation = Quaternion.Euler(targetRotBody);

        //xAxisClamp -= horizontal;

        //Vector3 targetRotBody = transform.rotation.eulerAngles;

        //transform.rotation = Quaternion.Euler(targetRotBody);

    }
    public void ThrottleBody()
    {
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirForward = Body.transform.forward * vertical * walkSpeed;
        characterController.SimpleMove(moveDirForward);
    }
    public void RotateTurret()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

        Vector3 targetRotTurret = Turret.transform.rotation.eulerAngles;
        //Vector3 targetRot

        //targetRotTurret.x -= rotAmountY;
        targetRotTurret.x = 0;
        targetRotTurret.z = 0;
        targetRotTurret.y += rotAmountX;

        Vector3 targetRotBarrel = Barrel.transform.rotation.eulerAngles;

        targetRotBarrel.x -= rotAmountY;
        targetRotBarrel.z = targetRotTurret.z;
        targetRotBarrel.y = targetRotTurret.y;
        //targetRotTurret.y += rotAmountX;

        if (xAxisClamp > xRotMinDown)
        {
            xAxisClamp = xRotMinDown;
            targetRotBarrel.x = xRotMinDown;
        }

        else if (xAxisClamp < xRotMaxUp)
        {
            xAxisClamp = xRotMaxUp;
            targetRotBarrel.x = xRotMaxUp;
        }

        Turret.transform.rotation = Quaternion.Euler(targetRotTurret);
        Barrel.transform.rotation = Quaternion.Euler(targetRotBarrel);
    }
    #endregion
}
