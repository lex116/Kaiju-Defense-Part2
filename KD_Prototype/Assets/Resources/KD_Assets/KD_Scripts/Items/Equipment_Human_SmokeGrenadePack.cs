using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Human_SmokeGrenadePack : Equipment_Master
{
    public Equipment_Human_SmokeGrenadePack()
    {
        Item_Name = "Smoke Grenades";
        EffectRadius = 12;
        EquipmentType = EquipmentTypes.Deployable;
    }

    public override void SetUp()
    {
        Ammo = 2;
        Projectile = (GameObject)(Resources.Load("KD_Assets/KD_Prefabs/SmokeGrenade"));
    }
}
