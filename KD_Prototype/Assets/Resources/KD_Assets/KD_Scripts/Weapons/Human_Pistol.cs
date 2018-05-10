using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Pistol : Weapon_Master
{
    public Human_Pistol()
    {
        Item_Name = "Pistol";
        Damage = 2;
        damageType = DamageType.Puncture;
        Accuracy = 85;
        ShotCount = 8;
        BurstCount = 1;
        Range = 100;
        FireRate = 0.25f;
        fireMode = FireModes.SingleShot;
        Weight = 1;
    }
}
