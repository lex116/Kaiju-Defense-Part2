﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleHardPoint : MonoBehaviour, IDamagable
{
    public string HardPointName;
    internal int StartingHitPoints;
    public int HitPoints;
    public Unit_VehicleMaster OwnerVehicle;
    internal Armor_Master AttachedArmor;

    public bool isDestroyed;
    RoundManager roundManager;

    void Awake()
    {
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
            int DamageToTake = 0;

            DamageToTake = Damage - (AttachedArmor.DamageResistance[(int)DamageType]);

            if (DamageToTake > 0)
            {
                HitPoints = HitPoints - DamageToTake;
            }

            if (HitPoints <= 0)
            {
                HitPoints = 0;
                DestroyHardPoint(Attacker);
            }
        }
    }

    public void DestroyHardPoint(string Attacker)
    {
        isDestroyed = true;

        OwnerVehicle.ChangeTeamNerve(-15);

        //NOTICE
        //roundManager = FindObjectOfType<RoundManager>();
        //roundManager.AddNotificationToFeed(Attacker + " destroyed " + HardPointName);

        Instantiate((Resources.Load<GameObject>("KD_Assets/KD_Prefabs/TempExplosion")), transform.position, transform.rotation);
    }
}