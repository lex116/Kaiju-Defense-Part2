using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Vehicle_Cannon : Weapon_Master 
{
    public Weapon_Vehicle_Cannon()
    {
        Item_Name = "Cannon";
        Damage = 25;
        damageType = DamageTypes.Explosive;
        Accuracy = 85;
        ShotCount = 1;
        BurstCount = 1;
        Range = 500;
        FireRate = 2f;
        fireMode = FireModes.AoeShot;
        Weight = 35;
        EffectRadius = 15;
    }

    public override void SetUp()
    {
        Firing_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Vehicle_Cannon_Shot"));
        Reload_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Vehicle_Cannon_Reload"));

        Reticle_Sprite = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_Cannon"));
        WeaponModel = (Resources.Load<GameObject>("KD_Assets/KD_Prefabs/Weapon_Human_AntiArmorRifle_Model"));
    }
}
