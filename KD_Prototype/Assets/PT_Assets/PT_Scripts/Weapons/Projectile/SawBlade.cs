using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : Projectile_Class
{
    float SpinRate = 200;

    void Update()
    {
        if(isAttachedDOT)
        {
            transform.Rotate(new Vector3(0, 1 * Time.deltaTime * SpinRate, 0));
        }
    }

    public override void SetUp()
    {
        ProType = ProjectileType.DOT;
        DudTime = 1f;
        Lifetime = 1f;
        Bounces = 2;
        Ticks = 3;
    }
}
