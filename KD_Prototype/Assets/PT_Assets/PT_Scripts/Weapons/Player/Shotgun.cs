using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "Shotgun";
        FactionTag = Faction.Player;
        DamageModifer = 4;
        projectileSpeed = 20;
        fireRate = 0.6f;
        ProjectileSpread = 4;
        maxAmmoCapacity = 6;
        reserveAmmo = 24;
        maxReserveAmmoCapacity = 54;
        reloadSpeed = 2;
        currentAmmo = maxAmmoCapacity;
    }
}
