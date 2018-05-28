using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_Shotgun : Weapon_Master
{
    public Weapon_Human_Shotgun()
    {
        Item_Name = "Shotgun";
        Damage = 2;
        damageType = DamageTypes.Shred;
        Accuracy = 60;
        ShotCount = 8;
        BurstCount = 2; 
        Range = 75;
        FireRate = .75f;
        fireMode = FireModes.SpreadShot;
        Weight = 3;
    }
}
