using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeGun : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "SawBlade Gun";
        FactionTag = Faction.Bandit;
        DamageModifer = 5;
        projectileSpeed = 10;
        fireRate = 1.5f;
        ProjectileSpread = 3;
        maxAmmoCapacity = 3;
        reserveAmmo = 12;
        maxReserveAmmoCapacity = 27;
        reloadSpeed = 1;
        currentAmmo = maxAmmoCapacity;
    }
}
