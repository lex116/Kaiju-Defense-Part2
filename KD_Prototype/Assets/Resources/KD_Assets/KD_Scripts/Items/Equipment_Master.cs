using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Master : Item_Master
{
    internal enum EquipmentTypes
    {
        SingleTarget,
        Deployable
    }

    internal EquipmentTypes EquipmentType;

    public int HealingValue;
    public int EffectDuration;

    public Transform DeployableSpawnLocation;
    public Unit_Master DeployableOwner;
    public int DeployableThrowForce;

    public virtual void UseEffect()
    {
        //
    }
}
