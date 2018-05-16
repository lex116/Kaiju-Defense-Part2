using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Mecha : Unit_VehicleMaster
{
    public Unit_VehicleHardPoint Sensor;
    public Unit_VehicleHardPoint RightArm;
    public Unit_VehicleHardPoint LeftArm;
    public Unit_VehicleHardPoint Legs;

    public override void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = 
            (characterSheet.UnitStat_Accuracy + equippedWeapon.Accuracy + CurrentPilot.characterSheet.UnitStat_Accuracy) / 3;

        if (CurrentPilot.isPanicked)
        {
            Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * 0.75f;
        }

        if (RightArm.isDestroyed)
        {
            Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * 0.25f;
        }

        if (Sensor.isDestroyed)
        {
            Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * 0.1f;
        }
    }

    public override void CalculateCarryWeight()
    {
        float CarryCapacity = characterSheet.UnitStat_Fitness;
        float CarryWeight = equippedWeapon.Weight + equippedEquipment.Weight + equippedArmor.Weight;
        float CarryWeightDifference = CarryCapacity - CarryWeight;
        float Encumberance = CarryWeightDifference / CarryCapacity;

        startingMovementPoints = characterSheet.UnitStat_Fitness;
        movementPointsRemaining = startingMovementPoints * Encumberance;

        if (Legs.isDestroyed == true)
        {
            movementPointsRemaining = movementPointsRemaining / 4;
        }
    }

    public override void Die(string Attacker)
    {
        ChangeTeamNerve(-25);

        //Debug.Log(this.gameObject.name + "has died");

        isDead = true;

        //if (roundManager.SelectedUnit == this)
        //{
        //    roundManager.EndUnitTurn();
        //}

        //this.transform.localScale = new Vector3(1f, 0.25f, 1f);
        //temp
        Destroy(KD_CC);

        foreach (Transform x in transform)
        {
            x.gameObject.AddComponent<Rigidbody>();
        }

        roundManager.AddNotificationToFeed(Attacker + " killed " + characterSheet.UnitStat_Name);
    }

    public override void ToggleControl(bool toggle)
    {
        playerCamera.gameObject.SetActive(toggle);
        IsBeingControlled = toggle;
        SetItems();
        SetAction(0);
        CalculateCarryWeight();

        if (toggle == false && CurrentPilot != null)
        {
            CurrentPilot.ToggleControl(false);
            //ToggleControl(false);
            Debug.Log("turn off vehicle");
        }
    }
}
