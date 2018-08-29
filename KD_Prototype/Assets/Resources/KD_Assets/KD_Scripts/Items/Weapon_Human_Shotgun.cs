using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_Shotgun : Weapon_Master
{
    public Weapon_Human_Shotgun()
    {
        Item_Name = "Shotgun";
        Damage = 3;
        damageType = DamageTypes.Shred;
        Accuracy = 60;
        ShotCount = 8;
        BurstCount = 2; 
        Range = 75;
        FireRate = .75f;
        fireMode = FireModes.SpreadShot;
        Weight = 3;
    }

    public override void SetUp()
    {
        Firing_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Shotgun_Shot"));
        Reload_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Shotgun_Reload"));

        Reticle_Sprite = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_Shotgun"));
        WeaponModel = (Resources.Load<GameObject>("KD_Assets/KD_Prefabs/Weapon_Human_Shotgun_Model"));
    }
}
