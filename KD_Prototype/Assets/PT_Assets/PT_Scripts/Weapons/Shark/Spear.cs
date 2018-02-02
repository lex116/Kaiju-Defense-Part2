using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Throwable_Class
{ 
    public override void SetUp()
    {
        weaponName = "Spear";
        FactionTag = Faction.Shark;
        ActivationTimer = 5;
        projectilesToFire = 1;
        DamageModifer = 100;
        projectileSpeed = 0;
        projectileSpread = 0;
        explodeOnContact = true;
        hasDespawnTimer = true;
        DespawnTimer = 10;
    }
}
