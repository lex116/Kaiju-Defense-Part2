using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "Rifle";
        FactionTag = Faction.Player;
        DamageModifer = 50;
        projectileSpeed = 60f;
        fireRate = 1.5f;
        ProjectileSpread = 0;
        maxAmmoCapacity = 4;
        reserveAmmo = 16;
        maxReserveAmmoCapacity = 36;
        reloadSpeed = 4;
        currentAmmo = maxAmmoCapacity;
    }
}
