using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Shotgun : Weapon_Master
{
    public Human_Shotgun()
    {
        Item_Name = "Shotgun";
        Damage = 1;
        damageType = DamageType.Puncture;
        Accuracy = 60;
        ShotCount = 10;
        BurstCount = 3; 
        Range = 75;
        FireRate = .75f;
        fireMode = FireModes.SpreadShot;
    }
}
