using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleHardPoint_Hull : Unit_VehicleHardPoint
{
    public override void SetUp()
    {
        HardPointName = "(" + OwnerVehicle.characterSheet.UnitStat_Name + ") " + HardPointName;
        StartingHitPoints = OwnerVehicle.characterSheet.UnitStat_StartingHitPoints;
        HitPoints = StartingHitPoints;
        AttachedArmor = OwnerVehicle.equippedArmor;
    }

    public override void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        OwnerVehicle.TakeDamage(Damage, DamageType, Attacker);
        HitPoints = OwnerVehicle.characterSheet.UnitStat_HitPoints;
    }
}
