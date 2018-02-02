 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [HideInInspector]
    int StartingHitPoints;

    //[HideInInspector]
    public float HitPoints;

    public bool isDestroyed = false;

	// Use this for initialization
	void Awake ()
    {
        Setup();
	}
	
	public void Setup()
    {
        //for now
        StartingHitPoints = 100;
        HitPoints = StartingHitPoints;
    }

    public virtual void TakeDamage(int Damage)
    {
        HitPoints = HitPoints - Damage;
        if (HitPoints <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (isDestroyed == false)
        {
            isDestroyed = true;
        }
    }
}
