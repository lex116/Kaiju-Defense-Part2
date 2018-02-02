using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRifle : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "AutoRifle";
        FactionTag = Faction.Robot;
        DamageModifer = 5;
        projectileSpeed = 30;
        fireRate = 0.25f;
        ProjectileSpread = 2;
        maxAmmoCapacity = 20;
        reserveAmmo = 80;
        maxReserveAmmoCapacity = 180;
        reloadSpeed = 2;
        currentAmmo = maxAmmoCapacity;
    }
}
