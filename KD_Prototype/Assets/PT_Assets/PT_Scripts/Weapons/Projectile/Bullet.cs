using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile_Class
{
    public override void SetUp()
    {
        ProType = ProjectileType.Richochet;
        DudTime = 1f;
        Lifetime = 1f;
        Bounces = 2;
    }
}

