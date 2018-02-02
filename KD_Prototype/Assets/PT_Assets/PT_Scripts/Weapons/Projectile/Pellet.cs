using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : Projectile_Class
{
    public override void SetUp()
    {
        DudTime = 1f;
        Lifetime = 1f;
        Bounces = 2;
    }
}
