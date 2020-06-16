using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_AI_PathingDummy : Unit_Master
{
    public KD_Global.FactionTag dummyTag;

    public override void Setup(KD_Global.Characters infantry, KD_Global.Characters vehicle)
    {
        //do nothing
    }

    public override void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        //Debug.Log(gameObject.name + " Hit");
    }
}
