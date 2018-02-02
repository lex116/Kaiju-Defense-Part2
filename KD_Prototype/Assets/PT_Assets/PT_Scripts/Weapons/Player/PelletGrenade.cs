using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletGrenade : Throwable_Class
{
    public override void SetUp()
    {
        weaponName = "Pellet Grenade";
        FactionTag = Faction.Player;
        ActivationTimer = 4;
        projectilesToFire = 30;
        DamageModifer = 20;
        projectileSpeed = 20;
        projectileSpread = 180;
        ThrowSpin = 0;
    }
}
