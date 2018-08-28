using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Basic_SuppressShot : Action_Master
{
    Action_Basic_SuppressShot()
    {
        Action_Name = "Suppress Shot";
        //Action_AP_Cost = 2;
        Action_AP_Cost = 1;
    }

    public override void SetUp()
    {
        Action_Icon = (Resources.Load<Sprite>("KD_Sprites/KD_ActionIcon_SuppressShot"));
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.roundManager.AddNotificationToFeed("Suppressing " + Action_Owner.suppressionTarget.characterSheet.UnitStat_Name + "!");
        Action_Owner.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_WaitingToSuppress;
        Action_Owner.ToggleMovingState();
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

        Action_Owner.PaintTarget();

        if (Action_Owner.suppressionTarget == null)
            return false;

        return true;
    }

    public override void Selection_Effect(Unit_Master Action_Owner)
    {
        //Action_Owner.roundManager.AddNotificationToFeed("Selected Suppress Shot!");
        Action_Owner.CurrentShotAccuracyModifier = Action_Owner.SuppressShotAccMod;
        Action_Owner.ScaleCameraFOV();
        Action_Owner.roundManager.Reticle.sprite = Action_Owner.equippedWeapon.Reticle_Sprite;
        Action_Owner.roundManager.Player_HUD_Shooting.SetActive(true);
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    {
        //Action_Owner.roundManager.AddNotificationToFeed("Deselected Suppress Shot!");
        Action_Owner.CurrentShotAccuracyModifier = 0;
        Action_Owner.ResetCameraFOV();
        Action_Owner.roundManager.Reticle.sprite = Action_Owner.Default_Reticle;
        Action_Owner.roundManager.Player_HUD_Shooting.SetActive(false);
    }
}
