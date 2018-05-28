using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Vehicle_Cannon : Weapon_Master 
{
    public Weapon_Vehicle_Cannon()
    {
        Item_Name = "Cannon";
        Damage = 15;
        damageType = DamageTypes.Explosive;
        Accuracy = 85;
        ShotCount = 1;
        BurstCount = 1;
        Range = 500;
        FireRate = 4f;
        fireMode = FireModes.AoeShot;
        EffectRadius = 25;
        Weight = 35;
    }
}
