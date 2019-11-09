using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Vehicle_MachineGun : Weapon_Master
{
    public Weapon_Vehicle_MachineGun()
    {
        Item_Name = "Heavy MachineGun";
        Damage = 8;
        damageType = DamageTypes.Puncture;
        Accuracy = 55;
        ShotCount = 10;
        BurstCount = 1;
        Range = 300;
        FireRate = 0.10f;
        fireMode = FireModes.SingleShot;
        Weight = 25;
    }

    public override void SetUp()
    {
        Firing_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Vehicle_MachineGun_Shot"));
        Reload_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Vehicle_MachineGun_Reload"));

        Reticle_Sprite = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_MachineGun"));
        WeaponModel = (Resources.Load<GameObject>("KD_Assets/KD_Prefabs/Weapon_Human_MachineGun_Model"));
    }
}
