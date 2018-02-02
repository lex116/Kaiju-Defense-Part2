using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile_Class
{
    Transform hitLocation;

    public override void SetUp()
    {
        ProType = ProjectileType.Attachment;
        DudTime = 15f;
        Lifetime = 2f;
        Bounces = 0;
    }
}
