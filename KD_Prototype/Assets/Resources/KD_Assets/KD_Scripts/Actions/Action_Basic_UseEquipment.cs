using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Basic_UseEquipment : Action_Master
{
    Action_Basic_UseEquipment()
    {
        Action_Name = "Use Equipment";
        Action_AP_Cost = 1;
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.equippedEquipment.UseEffect();
    }

    public override bool CheckRequirements(Unit_Master Action_Owner)
    {
        if (Action_Owner.equippedEquipment.Ammo <= 0)
            return false;

        Unit_VehicleMaster temp_VehicleMaster = Action_Owner as Unit_VehicleMaster;

        if (temp_VehicleMaster != null)
        {
            if (temp_VehicleMaster.SecondaryEquipment.isDestroyed == true)
                return false;
        }

        else
            return true;

        return true;
    }
}
