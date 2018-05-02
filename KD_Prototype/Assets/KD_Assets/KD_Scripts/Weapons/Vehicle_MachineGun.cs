using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_MachineGun : Weapon
{
    public Vehicle_MachineGun()
    {
        Weapon_Name = "Heavy MachineGun";
        Damage = 3;
        damageType = DamageType.Piercing;
        Accuracy = 70;
        ShotCount = 35;
        BurstCount = 1;
        Range = 300;
        FireRate = 0.10f;
        thisFireMode = FireModes.SingleShot;
    }
}
