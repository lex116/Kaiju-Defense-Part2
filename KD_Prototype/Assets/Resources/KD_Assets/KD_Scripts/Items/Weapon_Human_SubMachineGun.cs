using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Human_SubMachineGun : Weapon_Master
{
    public Weapon_Human_SubMachineGun()
    {
        Item_Name = "SubMachine Gun";
        Damage = 2;
        damageType = DamageTypes.Shred;
        Accuracy = 1;
        ShotCount = 30;
        BurstCount = 1;
        Range = 100;
        FireRate = 0.025f;
        fireMode = FireModes.SingleShot;
        Weight = 5;
    }

    public override void SetUp()
    {
        Firing_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Pistol_Shot"));
        Reload_Clip = (AudioClip)(Resources.Load("KD_SFX/Weapon_Human_Pistol_Reload"));

        Reticle_Sprite = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_MachineGun"));
        WeaponModel = (Resources.Load<GameObject>("KD_Assets/KD_Prefabs/Weapon_Human_Pistol_Model"));
    }
}
