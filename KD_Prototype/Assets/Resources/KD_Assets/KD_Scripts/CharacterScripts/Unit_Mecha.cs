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
        if (CurrentPilot_Character != null)
        {
            Calculated_WeaponAccuracy =
            (characterSheet.UnitStat_Accuracy + equippedWeapon.Accuracy + CurrentPilot_Character.UnitStat_Accuracy) / 3;

            if (CurrentPilot_Character.isPanicked)
            {
                Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * PanicAccMod;
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

        isDead = true;

        Destroy(KD_CC);

        foreach (Transform x in transform)
        {
            x.gameObject.AddComponent<Rigidbody>();
        }

        roundManager.AddNotificationToFeed(Attacker + " killed " + characterSheet.UnitStat_Name);
    }
}
