using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_MachineGun : Weapon
{
    public Human_MachineGun()
    {
        Weapon_Name = "MachineGun";
        Damage = 3;
        damageType = DamageType.Piercing;
        Accuracy = 80;
        ShotCount = 16;
        BurstCount = 1;
        Range = 150;
        FireRate = 0.30f;
        thisFireMode = FireModes.SingleShot;
    }
}
