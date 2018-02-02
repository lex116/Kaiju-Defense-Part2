using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hwacha : RangedWeapon_Class
{
    [SerializeField]
    GameObject[] ArrowModels;

    bool ModelsWaitingToAppear;

    void Update()
    {
        if (currentWeaponState != State.Ready && ModelsWaitingToAppear == false)
        {
            foreach (GameObject x in ArrowModels)
            {
                x.SetActive(false);
            }
            ModelsWaitingToAppear = true;
        }

        if (currentWeaponState == State.Ready && ModelsWaitingToAppear == true && currentAmmo > 0)
        {
            foreach (GameObject x in ArrowModels)
            {
                x.SetActive(true);
            }
            ModelsWaitingToAppear = false;
        }
    }

    public override void Setup()
    {
        weaponName = "Hwacha";
        FactionTag = Faction.Shark;
        DamageModifer = 25;
        projectileSpeed = 10f;
        fireRate = 1f;
        ProjectileSpread = 3;
        maxAmmoCapacity = 1;
        reserveAmmo = 4;
        maxReserveAmmoCapacity = 9;
        reloadSpeed = 10;
        currentAmmo = maxAmmoCapacity;
    }
}
