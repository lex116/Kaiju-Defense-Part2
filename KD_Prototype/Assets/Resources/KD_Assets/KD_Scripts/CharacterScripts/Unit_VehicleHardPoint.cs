using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleHardPoint : MonoBehaviour, IDamagable
{
    public string HardPointName;
    internal int StartingArmor = 20;
    public int Armor;

    public bool isDestroyed;
    RoundManager roundManager;

    void Awake()
    {
        Armor = StartingArmor;
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
        this.gameObject.AddComponent<Rigidbody>();
        this.gameObject.transform.parent = null;

        roundManager = FindObjectOfType<RoundManager>();
        roundManager.AddNotificationToFeed(Attacker + " destroyed " + this.name);
    }
}
