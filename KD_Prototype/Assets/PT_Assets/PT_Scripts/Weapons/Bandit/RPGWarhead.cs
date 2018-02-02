using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGWarhead : Throwable_Class
{
    public override void SetUp()
    {
        weaponName = "RPG Warhead";
        FactionTag = Faction.Bandit;
        ActivationTimer = 15;
        projectilesToFire = 30;
        DamageModifer = 35;
        projectileSpeed = 20;
        projectileSpread = 180;
        explodeOnContact = true;
    }
}
