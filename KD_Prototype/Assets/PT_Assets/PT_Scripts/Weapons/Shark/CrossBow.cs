using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBow : RangedWeapon_Class
{
    [SerializeField]
    GameObject ArrowModel;

    bool ModelWaitingToAppear;

    void Update()
    {
        if (currentWeaponState != State.Ready && ModelWaitingToAppear == false)
        {
            ArrowModel.SetActive(false);
            ModelWaitingToAppear = true;
        }

        if (currentWeaponState == State.Ready && ModelWaitingToAppear == true && currentAmmo > 0)
        {
            ArrowModel.SetActive(true);
            ModelWaitingToAppear = false;
        }
    }

    public override void Setup()
    {
        weaponName = "CrossBow";
        FactionTag = Faction.Bandit;
        DamageModifer = 40;
        projectileSpeed = 25f;
        fireRate = 1f;
        ProjectileSpread = 0;
        maxAmmoCapacity = 1;
        reserveAmmo = 4;
        maxReserveAmmoCapacity = 9;
        reloadSpeed = 3;
        currentAmmo = maxAmmoCapacity;
    }
}
