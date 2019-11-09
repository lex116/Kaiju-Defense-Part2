using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlord_Player : Overlord_Master
{
    public virtual void PlayerInput()
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

        if (Input.GetKeyDown(KeyCode.B) && characterController.isGrounded && ccu.Current_Unit_State == Unit_Master.Unit_States.State_Moving)
        {
            manager_HUD.HUD_Player_ConfirmText.SetActive(true);
            manager_GameState.EndUnitTurn();
        }

        if (Input.GetKeyDown(KeyCode.M))
            manager_HUD.ToggleMiniMap();

        if (Input.GetKeyDown(KeyCode.I))
            manager_HUD.ToggleInventoryScreen();

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
