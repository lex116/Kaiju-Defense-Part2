using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Projectile_Class
{
    Transform hitLocation;

    public override void SetUp()
    {
        ProType = ProjectileType.Attachment;
        DudTime = 1f;
        Lifetime = 2f;
        Bounces = 0;
    }
}
