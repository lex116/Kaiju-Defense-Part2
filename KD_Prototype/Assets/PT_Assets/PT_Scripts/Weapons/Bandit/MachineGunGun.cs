using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunGun : RangedWeapon_Class
{
    [SerializeField]
    GameObject Rotator;

    [SerializeField]
    GameObject[] AllTips;

    float CurrentSpinSpeed = 0f;
    float MaximumSpinSpeed = 200f;
    float SpinSpeedGrowth = 5;
    float SpinSpeedDecay = 10;

    void Update()
    {
        if (currentWeaponState == State.CoolingDown)
        {
            projectileOrigins[0] = AllTips[Random.Range(0, 4)];

            if (CurrentSpinSpeed < MaximumSpinSpeed)
            {
                CurrentSpinSpeed = CurrentSpinSpeed + SpinSpeedGrowth;
                if (CurrentSpinSpeed > MaximumSpinSpeed)
                {
                    CurrentSpinSpeed = MaximumSpinSpeed;
                }
            }

        }

        else
        {
            if (CurrentSpinSpeed > 0)
            {
                CurrentSpinSpeed = CurrentSpinSpeed - SpinSpeedDecay;
                if (CurrentSpinSpeed < 0)
                {
                    CurrentSpinSpeed = 0;
                }
            }
        }

        Rotator.transform.Rotate(new Vector3(0, 0, 1 * CurrentSpinSpeed * Time.deltaTime));
    }

    public override void Setup()
    {
        weaponName = "MachineGunGun";
        FactionTag = Faction.Bandit;
        DamageModifer = 4;
        projectileSpeed = 20;
        fireRate = 0.2f;
        ProjectileSpread = 3;
        maxAmmoCapacity = 120;
        reserveAmmo = 480;
        maxReserveAmmoCapacity = 1080;
        reloadSpeed = 10;
        currentAmmo = maxAmmoCapacity;
    }
}
