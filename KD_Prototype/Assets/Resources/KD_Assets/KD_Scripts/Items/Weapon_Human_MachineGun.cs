using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_MachineGun : Weapon_Master
{
    public Weapon_Human_MachineGun()
    {
        Item_Name = "MachineGun";
        Damage = 6;
        damageType = DamageTypes.Puncture;
        Accuracy = 65;
        ShotCount = 10;
        BurstCount = 1;
        Range = 150;
        FireRate = 0.1f;
        fireMode = FireModes.SingleShot;
        Weight = 4;
    }

    public override void SetUp()
    {
        Firing_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_MachineGun_Shot"));
        Reload_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_MachineGun_Reload"));

        Reticle_Sprite = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_MachineGun"));
        WeaponModel = (Resources.Load<GameObject>("KD_Assets/KD_Prefabs/Weapon_Human_MachineGun_Model"));
    }
}
