using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleHardPoint : MonoBehaviour, IDamagable
{

    public string Name;
    int StartingArmor = 20;
    int Armor;
    int DamageResist = 1;
    bool isDestroyed;
    RoundManager roundManager;

    void Awake()
    {
        Armor = StartingArmor;
    }

    public void TakeDamage(int Damage, string Attacker)
    {
        if (isDestroyed == false)
        {
            Damage = Damage - DamageResist;

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

        roundManager = FindObjectOfType<RoundManager>();
        roundManager.AddNotificationToFeed(Attacker + " destroyed " + this.name);
    }
}
