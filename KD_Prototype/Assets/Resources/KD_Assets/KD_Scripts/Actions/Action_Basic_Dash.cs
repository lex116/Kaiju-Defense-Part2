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

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.ResetMovement();
        Action_Owner.CalculateCarryWeight();
        Action_Owner.Current_Unit_State = Unit_Master.Unit_States.State_Moving;
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
        Action_Owner.roundManager.AddNotificationToFeed("Selected Dash!");
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Deselected Dash!");
    }
}
