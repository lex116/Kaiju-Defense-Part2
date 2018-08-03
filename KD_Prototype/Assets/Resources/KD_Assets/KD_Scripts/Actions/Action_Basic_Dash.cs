using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Basic_Dash : Action_Master
{
    Action_Basic_Dash()
    {
        Action_Name = "Dash";
        Action_AP_Cost = 1;
    }

    public override void SetUp()
    {
        Action_Icon = (Resources.Load<Sprite>("KD_Sprites/KD_ActionIcon_Dash"));
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.ResetMovement();
        Action_Owner.CalculateCarryWeight();
        Action_Owner.ToggleMovingState();
    }

    public override bool CheckRequirements(Unit_Master Action_Owner)
    {
        Unit_VehicleMaster temp_VehicleMaster = Action_Owner as Unit_VehicleMaster;

        if (temp_VehicleMaster != null)
        {
            if (temp_VehicleMaster.Locomotion.isDestroyed == true)
                return false;
        }

        else
            return true;

        return true;
    }

    public override void Selection_Effect(Unit_Master Action_Owner)
    {
        //Action_Owner.roundManager.AddNotificationToFeed("Selected Dash!");
        Action_Owner.roundManager.Player_HUD_Moving.SetActive(true);
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    {
        //Action_Owner.roundManager.AddNotificationToFeed("Deselected Dash!");
        Action_Owner.roundManager.Player_HUD_Moving.SetActive(false);
    }
}
