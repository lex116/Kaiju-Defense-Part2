using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_Pistol : Weapon_Master
{
    public Weapon_Human_Pistol()
    {
        Item_Name = "Pistol";
        Damage = 2;
        damageType = DamageTypes.Puncture;
        Accuracy = 85;
        ShotCount = 8;
        BurstCount = 1;
        Range = 100;
        FireRate = 0.25f;
        fireMode = FireModes.SingleShot;
        Weight = 1;
    }
}
