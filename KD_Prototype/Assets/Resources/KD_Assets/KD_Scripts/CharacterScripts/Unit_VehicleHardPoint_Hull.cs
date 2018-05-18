using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleHardPoint_Hull : Unit_VehicleHardPoint
{
    public override void SetUp()
    {
        HardPointName = "(" + OwnerVehicle.characterSheet.UnitStat_Name + ") " + HardPointName;
        StartingArmor = OwnerVehicle.characterSheet.UnitStat_StartingHitPoints;
        Armor = OwnerVehicle.characterSheet.UnitStat_HitPoints;
    }

    public override void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        OwnerVehicle.TakeDamage(Damage, DamageType, Attacker);
        Armor = OwnerVehicle.characterSheet.UnitStat_HitPoints;
    }
}
