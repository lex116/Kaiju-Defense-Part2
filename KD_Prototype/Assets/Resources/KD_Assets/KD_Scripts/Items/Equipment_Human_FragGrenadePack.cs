using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Human_FragGrenadePack : Equipment_Master
{
    public Equipment_Human_FragGrenadePack()
    {
        Item_Name = "Frag Grenades";
        EffectRadius = 10;
        Damage = 10;
        damageType = DamageTypes.Explosive;
        EquipmentType = EquipmentTypes.Deployable;
    }

    public override void SetUp()
    {
        Ammo = 2;
        Projectile = (GameObject)(Resources.Load("KD_Assets/KD_Prefabs/FragGrenade"));
    }
}
