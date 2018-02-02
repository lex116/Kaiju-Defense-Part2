using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : Throwable_Class
{
    public override void SetUp()
    {
        weaponName = "Molotov";
        FactionTag = Faction.Fire;
        ActivationTimer = 10;
        projectilesToFire = 15;
        DamageModifer = 1;
        projectileSpeed = 1;
        projectileSpread = 180;
        ThrowSpin = 1;
        explodeOnContact = true;
    }
}
