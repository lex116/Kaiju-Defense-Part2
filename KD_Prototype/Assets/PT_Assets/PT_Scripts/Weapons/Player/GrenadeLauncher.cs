using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "Grenade Launcher";
        FactionTag = Faction.Player;
        projectileSpeed = 10;
        fireRate = 1.25f;
        ProjectileSpread = 3;
        maxAmmoCapacity = 10;
        reserveAmmo = 40;
        maxReserveAmmoCapacity = 90;
        reloadSpeed = 4;
        currentAmmo = maxAmmoCapacity;
    }
}
