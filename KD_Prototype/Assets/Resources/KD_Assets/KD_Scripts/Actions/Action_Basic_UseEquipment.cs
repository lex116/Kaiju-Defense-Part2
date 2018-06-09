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

    public override void SetUp()
    {
        Action_Icon = (Resources.Load<Sprite>("KD_Sprites/KD_ActionIcon_UseEquipment"));
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.Current_Unit_State = Unit_Master.Unit_States.State_UsingEquipment;

        if (Action_Owner.equippedEquipment.EquipmentType == Equipment_Master.EquipmentTypes.Deployable)
        {
            Action_Owner.throwing.Projectile = Action_Owner.equippedEquipment.Projectile;
            Action_Owner.throwing.LaunchProjectile();
        }

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

        if (Action_Owner.equippedEquipment.EquipmentType == Equipment_Master.EquipmentTypes.Deployable)
        {
            if (Action_Owner.throwing.CanThrow == false)
                return false;
        }

        return true;
    }

    public override void Selection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Selected Use Equipment!");

        if (Action_Owner.equippedEquipment.EquipmentType == Equipment_Master.EquipmentTypes.Deployable)
        {
            Action_Owner.throwing.isTargetting = true;
            Action_Owner.throwing.ToggleTargettingGraphics(true);
        }
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Deselected Use Equipment");

        if (Action_Owner.equippedEquipment.EquipmentType == Equipment_Master.EquipmentTypes.Deployable)
        {
            Action_Owner.throwing.isTargetting = false;
            Action_Owner.throwing.ToggleTargettingGraphics(false);
        }
    }
}
