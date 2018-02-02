using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "Pistol";
        FactionTag = Faction.Player;
        DamageModifer = 8;
        projectileSpeed = 20f;
        fireRate = 0.35f;
        ProjectileSpread = 1;
        maxAmmoCapacity = 15;
        reserveAmmo = 60;
        maxReserveAmmoCapacity = 135;
        reloadSpeed = 2;
        currentAmmo = maxAmmoCapacity;
    }
}
