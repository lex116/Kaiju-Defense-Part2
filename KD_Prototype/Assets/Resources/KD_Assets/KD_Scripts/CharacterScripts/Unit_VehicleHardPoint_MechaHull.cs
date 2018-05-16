using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleHardPoint_MechaHull : Unit_VehicleHardPoint
{
    public Unit_Mecha Mecha;
    
    Unit_VehicleHardPoint_MechaHull()
    {
        StartingArmor = 0;
    }

    public void Start()
    {
        Mecha = GetComponentInParent<Unit_Mecha>();
    }

    public override  void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        Mecha.TakeDamage(Damage, DamageType, Attacker);
    }
}
