using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomahawkGrenade : Throwable_Class
{
    public override void SetUp()
    {
        weaponName = "Tomahawk Grenade";
        FactionTag = Faction.Bandit;
        ActivationTimer = 5;
        projectilesToFire = 10;
        DamageModifer = 45;
        projectileSpeed = 15;
        projectileSpread = 180;
        ThrowSpin = 1;
    }
}
