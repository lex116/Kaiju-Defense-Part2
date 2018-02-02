using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : RangedWeapon_Class
{
    [SerializeField]
    GameObject RocketModel;

    bool ModelWaitingToAppear;

    void Update()
    {
        if (currentWeaponState != State.Ready && ModelWaitingToAppear == false)
        {
            RocketModel.SetActive(false);
            ModelWaitingToAppear = true;
        }

        if (currentWeaponState == State.Ready && ModelWaitingToAppear == true && currentAmmo > 0)
        {
            RocketModel.SetActive(true);
            ModelWaitingToAppear = false;
        }
    }

    public override void Setup()
    {
        weaponName = "RPG";
        FactionTag = Faction.Bandit;
        projectileSpeed = 10;
        fireRate = 1f;
        ProjectileSpread = 2;
        maxAmmoCapacity = 1;
        reserveAmmo = 4;
        maxReserveAmmoCapacity = 9;
        reloadSpeed = 5;
        currentAmmo = maxAmmoCapacity;
    }
}
