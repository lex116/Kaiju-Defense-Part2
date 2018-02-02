using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinePistol : RangedWeapon_Class
{
    public override void Setup()
    {
        weaponName = "Machine Pistol";
        FactionTag = Faction.Player;
        DamageModifer = 4;
        projectileSpeed = 20;
        fireRate = 0.15f;
        ProjectileSpread = 3;
        maxAmmoCapacity = 35;
        reserveAmmo = 140;
        maxReserveAmmoCapacity = 315;
        reloadSpeed = 2;
        currentAmmo = maxAmmoCapacity;
    }
}
