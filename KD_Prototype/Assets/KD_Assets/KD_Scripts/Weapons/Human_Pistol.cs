using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Pistol : Weapon
{
    public Human_Pistol()
    {
        Weapon_Name = "Pistol";
        Damage = 2;
        damageType = DamageType.Piercing;
        Accuracy = .97f;
        ShotCount = 8;
        BurstCount = 1;
        Range = 100;
        FireRate = 0.25f;
        thisFireMode = FireModes.SingleShot;
    }
}
