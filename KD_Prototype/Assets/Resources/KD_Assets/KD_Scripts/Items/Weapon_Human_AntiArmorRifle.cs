using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_AntiArmorRifle : Weapon_Master
{
    public Weapon_Human_AntiArmorRifle()
    {
        Item_Name = "Anti-Armor Rifle";
        Damage = 25;
        damageType = DamageTypes.Puncture;
        Accuracy = 95;
        ShotCount = 1;
        BurstCount = 1;
        Range = 1000;
        FireRate = 1f;
        fireMode = FireModes.SingleShot;
        Weight = 6;
    }

    public override void SetUp()
    {
        Firing_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Rifle_Shot"));
        Reload_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Rifle_Reload"));

        Reticle_Sprite = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_Rifle"));
    }
}
