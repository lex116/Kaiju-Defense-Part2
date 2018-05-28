using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_AntiArmorRifle : Weapon_Master
{
    public Weapon_Human_AntiArmorRifle()
    {
        Item_Name = "Anti-Armor Rifle";
        Damage = 20;
        damageType = DamageTypes.Puncture;
        Accuracy = 95;
        ShotCount = 1;
        BurstCount = 1;
        Range = 1000;
        FireRate = 1f;
        fireMode = FireModes.SingleShot;
        Weight = 6;
    }
}
