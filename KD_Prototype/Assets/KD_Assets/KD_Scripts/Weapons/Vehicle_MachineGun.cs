using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_MachineGun : Weapon_Master
{
    public Vehicle_MachineGun()
    {
        Item_Name = "Heavy MachineGun";
        Damage = 3;
        damageType = DamageType.Puncture;
        Accuracy = 70;
        ShotCount = 35;
        BurstCount = 1;
        Range = 300;
        FireRate = 0.10f;
        fireMode = FireModes.SingleShot;
    }
}
