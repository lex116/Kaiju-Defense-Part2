using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Shotgun : Weapon
{
    public Human_Shotgun()
    {
        Weapon_Name = "Shotgun";
        Damage = 1;
        damageType = DamageType.Piercing;
        Accuracy = 15;
        ShotCount = 10;
        BurstCount = 3; 
        Range = 75;
        FireRate = .75f;
        thisFireMode = FireModes.SpreadShot;
    }
}
