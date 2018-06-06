using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Basic_AimedShot : Action_Master
{
    Action_Basic_AimedShot()
    {
        Action_Name = "Aimed Shot";
        Action_AP_Cost = 2;
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.Current_Unit_State = Unit_Master.Unit_States.State_Shooting;
        Action_Owner.shooting.TestShooting(Action_Owner.AimedShotAccMod);
        Action_Owner.equippedWeapon.Ammo--;
    }

    public override bool CheckRequirements(Unit_Master Action_Owner)
    {
        if (Action_Owner.equippedWeapon.Ammo <= 0)
            return false;

        Unit_VehicleMaster temp_VehicleMaster = Action_Owner as Unit_VehicleMaster;

        if (temp_VehicleMaster != null)
        {
            if (temp_VehicleMaster.PrimaryWeapon.isDestroyed == true)
                return false;
        }

        else
            return true;

        return true;
    }

    public override void Selection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Selected Aimed Shot!");
        Action_Owner.CurrentShotAccuracyModifier = Action_Owner.AimedShotAccMod;
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    { 
        Action_Owner.roundManager.AddNotificationToFeed("Deselected Aimed Shot!");
        Action_Owner.CurrentShotAccuracyModifier = 0;
    }
}