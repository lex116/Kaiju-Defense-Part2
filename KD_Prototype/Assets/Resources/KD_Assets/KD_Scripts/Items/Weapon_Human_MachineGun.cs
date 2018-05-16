using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_MachineGun : Weapon_Master
{
    public Weapon_Human_MachineGun()
    {
        Item_Name = "MachineGun";
        Damage = 3;
        damageType = DamageTypes.Puncture;
        Accuracy = 80;
        ShotCount = 16;
        BurstCount = 1;
        Range = 150;
        FireRate = 0.30f;
        fireMode = FireModes.SingleShot;
        Weight = 3;
    }
}
