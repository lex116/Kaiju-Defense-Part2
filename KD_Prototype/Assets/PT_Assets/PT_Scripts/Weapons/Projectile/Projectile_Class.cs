using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Class : MonoBehaviour
{
    #region Fields
    [HideInInspector]
    public float Lifetime;
    [HideInInspector]
    public float DudTime;
    public bool isDud = true;
    [HideInInspector]
    public int ProjectileDamage = 1;
    [HideInInspector]
    public Rigidbody projectileRigidbody;
    public Collider projectileCollider;
    [HideInInspector]
    public GameObject ObjectHit;
    public IDamagable ObjectToDamage;
    [HideInInspector]
    public int Bounces;
    public bool isAttachedDOT;
    float TickSpeed = 0.5f;
    public int Ticks;
    public enum ProjectileType
    {
        Richochet,
        Attachment,
        DOT
    }
    [HideInInspector]
    public ProjectileType ProType;
    #endregion

    // Use this for initialization
    void Awake()
    {
        SetUp();
        projectileRigidbody = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
        StartCoroutine(DudTimer());
    }

    public virtual void SetUp()
    {

    }

    public IEnumerator DudTimer()
    {
        yield return new WaitForSeconds(DudTime);

        if (isDud == true)
        {
            KillSelf();
        }
        if (isDud == false && isAttachedDOT == false)
        {
            StartCoroutine(Lifetimer());
        }
    }

    public IEnumerator Lifetimer()
    {
        yield return new WaitForSeconds(Lifetime);
        KillSelf();
    }

    public void KillSelf()
    {
        Destroy(gameObject);
    }

    public void Bounced()
    {
        Bounces = Bounces - 1;
        if (Bounces <= 0)
        {
            BounceEffect();
        }
    }

    public virtual void BounceEffect()
    {

    }

    public void HitSomething()
    {
        isDud = false;
        Bounced();
    }

    public IEnumerator DamageOverTime(int ticksToDo)
    {
        for (int i = 0; i < ticksToDo; i++)
        {
            if (ObjectToDamage != null)
            {
                ObjectToDamage.TakeDamage(ProjectileDamage);
            }
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(Lifetimer());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ProType == ProjectileType.Richochet)
        {
            RichochetCollisionFunction(collision);
        }

        if (ProType == ProjectileType.Attachment)
        {
            AttachCollisionFunction(collision);
        } 
    }

    public void RichochetCollisionFunction(Collision collision)
    {
        HitSomething();

        if (!isDud)
        {
            projectileRigidbody.useGravity = true;

            ObjectHit = collision.gameObject;
            ObjectToDamage = ObjectHit.GetComponent<IDamagable>();
            if (ObjectToDamage != null)
            {
                ObjectToDamage.TakeDamage(ProjectileDamage);
                KillSelf();
            }
        }
    }

    public void AttachCollisionFunction(Collision collision)
    {
        HitSomething();

        if (!isDud)
        {
            Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();

            if (hitRb != null)
            {
                ObjectHit = collision.gameObject;

                ContactPoint contact = collision.contacts[0];

                ObjectToDamage = ObjectHit.GetComponent<IDamagable>();

                if (ObjectToDamage != null)
                {
                    ObjectToDamage.TakeDamage(ProjectileDamage);
                }
            }
        }
    }
}
