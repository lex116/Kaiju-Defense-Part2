using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGrenade : Throwable_Class
{
    public override void SetUp()
    {
        weaponName = "Spike Grenade";
        FactionTag = Faction.Bandit;
        ActivationTimer = 2;
        projectilesToFire = 10;
        DamageModifer = 45;
        projectileSpeed = 15;
        projectileSpread = 180;
        ThrowSpin = 1;
    }
}
