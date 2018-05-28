using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Vehicle_MachineGun : Weapon_Master
{
    public Weapon_Vehicle_MachineGun()
    {
        Item_Name = "Heavy MachineGun";
        Damage = 8;
        damageType = DamageTypes.Puncture;
        Accuracy = 70;
        ShotCount = 12;
        BurstCount = 1;
        Range = 300;
        FireRate = 0.10f;
        fireMode = FireModes.SingleShot;
        Weight = 25;
    }
}
