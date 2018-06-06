using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Basic_SuppressShot : Action_Master
{
    Action_Basic_SuppressShot()
    {
        Action_Name = "Suppress Shot";
        Action_AP_Cost = 2;
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.PaintTarget();
        Action_Owner.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_WaitingToSuppress;
    }

    public override bool CheckRequirements(Unit_Master Action_Owner)
    {
        if (Action_Owner.equippedWeapon.Ammo <= 0)
            return false;

        Unit_VehicleMaster temp_VehicleMaster = Action_Owner as Unit_VehicleMaster;

        if (temp_VehicleMaster != null)
        {
            if (temp_VehicleMaster.Sensor.isDestroyed == true)
                return false;
        }

        if (Action_Owner.suppressionTarget == null)
            return false;

        return true;
    }

    public override void Selection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Selected Suppress Shot!");
        Action_Owner.CurrentShotAccuracyModifier = Action_Owner.SuppressShotAccMod;
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Deselected Suppress Shot!");
        Action_Owner.CurrentShotAccuracyModifier = 0;
    }
}
