using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkPupLauncher : RangedWeapon_Class
{
    [SerializeField]
    GameObject SharkModel;

    bool ModelWaitingToAppear;

    void Update()
    {
        if (currentWeaponState == State.Ready && ModelWaitingToAppear == true && currentAmmo > 0)
        {
            SharkModel.SetActive(true);
            ModelWaitingToAppear = false;
        }

        if ((currentWeaponState == State.CoolingDown && ModelWaitingToAppear == false) ||
            (currentWeaponState == State.Reloading && ModelWaitingToAppear == false) || currentAmmo <= 0)
        {
            SharkModel.SetActive(false);
            ModelWaitingToAppear = true;
        }
    }

    public override void Setup()
    {
        weaponName = "Shark Launcher";
        FactionTag = Faction.Shark;
        projectileSpeed = 15;
        fireRate = 1.5f;
        ProjectileSpread = 2;
        maxAmmoCapacity = 4;
        reserveAmmo = 16;
        maxReserveAmmoCapacity = 36;
        reloadSpeed = 4;
        currentAmmo = maxAmmoCapacity;
    }
}
