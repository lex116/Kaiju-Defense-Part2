﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Basic_QuickShot : Action_Master
{
    Action_Basic_QuickShot()
    {
        Action_Name = "Quick Shot";
        Action_AP_Cost = 1;
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.Current_Unit_State = Unit_Master.Unit_States.State_Shooting;
        Action_Owner.shooting.TestShooting(Action_Owner.QuickShotAccMod);
        Action_Owner.equippedWeapon.Ammo--;
    }

    public override bool CheckRequirements(Unit_Master Action_Owner)
    {
        if (Action_Owner.equippedWeapon.Ammo <= 0)
            return false;

        else
            return true;
    }

    public override void Selection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Selected Quick Shot!");
        Action_Owner.CurrentShotAccuracyModifier = Action_Owner.QuickShotAccMod;
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Deselected Quick Shot");
        Action_Owner.CurrentShotAccuracyModifier = 0;
    }
}