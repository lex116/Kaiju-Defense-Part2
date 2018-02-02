using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MassDriver : RangedWeapon_Class
{
    public float Charge;
    float ChargeRate = 0.75f;
    float RotationSpeed;
    int damageModl = 80;
    float resetSpeed = 5;

    public GameObject[] Spinners;

    void FixedUpdate()
    {
        if (currentWeaponState == State.Ready)
        {
            if (Charge < 100)
            {
                Charge = Charge + ChargeRate;
            }
        }

        RotationSpeed = 2.5f * currentAmmo;

        foreach (GameObject x in Spinners)
        {
            int index = Array.IndexOf(Spinners, x) + 1;

            if (Charge > ((index * 10) + 10))
            {
                x.transform.Rotate(0, 0, 1 * RotationSpeed);
            }
        }

        if (currentWeaponState != State.Ready)
        {
            Charge = 0;
        }

        DamageModifer = Mathf.RoundToInt(damageModl * (Charge / 100));

        if (currentWeaponState != State.Ready)
        {
            foreach (GameObject x in Spinners)
            {
                x.transform.localRotation =
                    Quaternion.Slerp(x.transform.localRotation, Quaternion.Euler(0, 0, 0), resetSpeed * Time.deltaTime);
            }
        }
    }

    public override void Setup()
    {
        weaponName = "Mass Driver";
        FactionTag = Faction.Robot;
        DamageModifer = 80;
        projectileSpeed = 20f;
        fireRate = 1;
        ProjectileSpread = 0;
        maxAmmoCapacity = 2;
        reserveAmmo = 8;
        maxReserveAmmoCapacity = 18;
        reloadSpeed = 5;
        currentAmmo = maxAmmoCapacity;
    }
}
