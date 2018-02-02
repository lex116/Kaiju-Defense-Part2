using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NapalmThrower : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "Napalm Thrower";
        FactionTag = Faction.Fire;
        DamageModifer = 1;
        projectileSpeed = 10;
        fireRate = 1f;
        ProjectileSpread = 3;
        maxAmmoCapacity = 4;
        reserveAmmo = 16;
        maxReserveAmmoCapacity = 36;
        reloadSpeed = 3;
        currentAmmo = maxAmmoCapacity;
    }
}
