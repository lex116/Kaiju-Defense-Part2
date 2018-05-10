using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_AntiArmorRifle : Weapon_Master
{
    public Human_AntiArmorRifle()
    {
        Item_Name = "Anti-Armor Rifle";
        Damage = 10;
        damageType = DamageType.Puncture;
        Accuracy = 50;
        ShotCount = 1;
        BurstCount = 1;
        Range = 1000;
        FireRate = 1f;
        fireMode = FireModes.SingleShot;
        Weight = 6;
    }
}
