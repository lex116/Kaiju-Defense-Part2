using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlord_Player : Overlord_Master
{
    internal float mouseSensitivity = 1;

    public override void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (tempOverwrite != 0)
            vertical = tempOverwrite;

        Vector3 moveDirSide = ccu.transform.right * horizontal * walkSpeed;
        Vector3 moveDirForward = ccu.transform.forward * vertical * walkSpeed;

        ccu_CharacterController.SimpleMove(moveDirSide * Time.deltaTime);
        ccu_CharacterController.SimpleMove(moveDirForward * Time.deltaTime);
    }

    public override void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

        Vector3 targetRotCam = ccu_aimingNode.transform.rotation.eulerAngles;
        Vector3 targetRotBody = ccu.transform.rotation.eulerAngles;

        targetRotCam.x -= rotAmountY;
        targetRotCam.z = 0;

        targetRotBody.y += rotAmountX;

        if (xAxisClamp > xRotMinDown)
        {
            xAxisClamp = xRotMinDown;
            targetRotCam.x = xRotMinDown;
        }

        else if (xAxisClamp < xRotMaxUp)
        {
            xAxisClamp = xRotMaxUp;
            targetRotCam.x = -3 * xRotMaxUp;
        }

        ccu_aimingNode.transform.rotation = Quaternion.Euler(targetRotCam);
        ccu.transform.rotation = Quaternion.Euler(targetRotBody);
    }

    public override void OverlordInput()
    {
        //Switch between moving and firing mode
        if (Input.GetKeyDown(KeyCode.Mouse1))
            ToggleMovingState();

        if (ccu.Current_Unit_State == Unit_Master.Unit_States.State_PreparingToAct)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                UseSelectedAction();

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                ChangeAction(1);

            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                ChangeAction(2);

            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                ChangeAction(3);

            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                ChangeAction(4);

            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                ChangeAction(5);

            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                ChangeAction(6);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                manager_HUD.ToggleExitApplicationCanvas();
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && ccu_CharacterController.isGrounded 
            && ccu.Current_Unit_State == Unit_Master.Unit_States.State_Moving)
        {
            manager_HUD.HUD_Player_ConfirmText.SetActive(true);
            manager_GameState.EndUnitTurn();
        }

        if (Input.GetKeyDown(KeyCode.M))
            manager_HUD.ToggleMiniMap();

        if (Input.GetKeyDown(KeyCode.I))
            manager_HUD.ToggleInventoryScreen();

        //this is just a testing thing, delete this after finishing up with the AI
        if (Input.GetKeyDown(KeyCode.J))
            tempOverwrite = 1;

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    throwing.isTargetting = true;
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //    Interaction();
    }
}
