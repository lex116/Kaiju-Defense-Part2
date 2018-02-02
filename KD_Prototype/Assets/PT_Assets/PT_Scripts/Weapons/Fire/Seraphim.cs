using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seraphim : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "Seraphim";
        FactionTag = Faction.Fire;
        DamageModifer = 10;
        projectileSpeed = 100f;
        fireRate = 1f;
        ProjectileSpread = 0;
        maxAmmoCapacity = 1;
        reserveAmmo = 4;
        maxReserveAmmoCapacity = 9;
        reloadSpeed = 7;
        currentAmmo = maxAmmoCapacity;
    }
}
