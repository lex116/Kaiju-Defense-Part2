using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassRail : Projectile_Class
{
    Transform hitLocation;

    public override void SetUp()
    {
        ProType = ProjectileType.Richochet;
        DudTime = 20f;
        Lifetime = 2f;
        Bounces = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        ObjectHit = other.gameObject;
        ObjectToDamage = ObjectHit.GetComponent<IDamagable>();
        if (ObjectToDamage != null)
        {
            ObjectToDamage.TakeDamage(ProjectileDamage);
        }
    }
}
