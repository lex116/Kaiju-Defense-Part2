using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeRifle : RangedWeapon_Class
{
    float MinimumFireRate = 0f;
    float MaximumFireRate = 1f;
    float MinimumProjectileSpread = 0;
    float MaximumProjectileSpread = 4;
    float MinimumProjectileSpeed = 10;
    float MaximumProjectileSpeed = 30;

    public bool WaitingToRandomize;

    void Update()
    {
        if (currentWeaponState == State.CoolingDown && WaitingToRandomize == true)
        {
            fireRate = Random.Range(MinimumFireRate, MaximumFireRate);
            ProjectileSpread = Random.Range(MinimumProjectileSpread, MaximumProjectileSpread);
            projectileSpeed = Random.Range(MinimumProjectileSpeed, MaximumProjectileSpeed);

            WaitingToRandomize = false;
        }

        if (currentWeaponState == State.Ready && WaitingToRandomize == false)
        {
            WaitingToRandomize = true;
        }
    }

    public override void Setup()
    {
        weaponName = "Pipe Rifle";
        FactionTag = Faction.Fire;
        DamageModifer = 1;
        projectileSpeed = 20;
        fireRate = 0.15f;
        ProjectileSpread = 3;
        maxAmmoCapacity = 15;
        reserveAmmo = 60;
        maxReserveAmmoCapacity = 135;
        reloadSpeed = 2;
        currentAmmo = maxAmmoCapacity;
    }
}
