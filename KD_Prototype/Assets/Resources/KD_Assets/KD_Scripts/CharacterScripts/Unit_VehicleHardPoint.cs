using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleHardPoint : MonoBehaviour, IDamagable
{
    public string HardPointName;
    internal int StartingArmor = 50;
    internal int Armor;
    public Unit_VehicleMaster OwnerVehicle;

    public bool isDestroyed;
    RoundManager roundManager;

    void Awake()
    {
        Armor = StartingArmor;
        OwnerVehicle = GetComponentInParent<Unit_VehicleMaster>();
    }

    void Start()
    {
        SetUp();
    }

    public virtual void SetUp()
    {
        HardPointName = "(" + OwnerVehicle.characterSheet.UnitStat_Name + ") " + HardPointName;
    }

    public virtual void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        if (isDestroyed == false)
        {
            if (Damage > 0)
            {
                Armor = Armor - Damage;
            }

            if (Armor <= 0)
            {
                Armor = 0;
                DestroyHardPoint(Attacker);
            }
        }
    }

    void DestroyHardPoint(string Attacker)
    {
        isDestroyed = true;

        roundManager = FindObjectOfType<RoundManager>();
        roundManager.AddNotificationToFeed(Attacker + " destroyed " + HardPointName);
    }
}
