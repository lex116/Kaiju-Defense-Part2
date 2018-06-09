using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_Pistol : Weapon_Master
{
    public Weapon_Human_Pistol()
    {
        Item_Name = "Pistol";
        Damage = 5;
        damageType = DamageTypes.Puncture;
        Accuracy = 85;
        ShotCount = 5;
        BurstCount = 1;
        Range = 100;
        FireRate = 0.25f;
        fireMode = FireModes.SingleShot;
        Weight = 1;
    }

    public override void SetUp()
    {
        Firing_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Pistol_Shot"));
        Reload_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Pistol_Reload"));

        Reticle_Sprite = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_Pistol"));
    }
}
