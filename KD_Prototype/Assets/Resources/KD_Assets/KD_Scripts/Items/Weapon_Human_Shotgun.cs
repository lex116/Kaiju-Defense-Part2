using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_Shotgun : Weapon_Master
{
    public Weapon_Human_Shotgun()
    {
        Item_Name = "Shotgun";
        Damage = 1;
        damageType = DamageTypes.Puncture;
        Accuracy = 60;
        ShotCount = 10;
        BurstCount = 3; 
        Range = 75;
        FireRate = .75f;
        fireMode = FireModes.SpreadShot;
        Weight = 3;
    }
}
