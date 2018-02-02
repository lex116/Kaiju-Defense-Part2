using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable_Class : MasterWeapon_Class
{
    #region Fields

    [HideInInspector]
    public float ActivationTimer;

    [HideInInspector]
    public int projectilesToFire;

    [HideInInspector]
    public float projectileSpread;

    [HideInInspector]
    public Rigidbody ThrowableRigidbody;
    float RigidBodyMassToStore;

    [HideInInspector]
    public int ThrowSpin;

    [SerializeField]
    Collider AttacherCollider;

    public bool explodeOnContact;
    public bool EOC_armed;

    Collider ThrowerToIgnore;

    public float DespawnTimer;
    public bool hasDespawnTimer;

    bool Armed;
    public bool waitingToDie;
    #endregion

    #region Functions

    // Use this for initialization
    void Awake ()
    {
        SetUp();
        ThrowableRigidbody = GetComponent<Rigidbody>();
        InteractionCollider = GetComponent<BoxCollider>();
        RigidBodyMassToStore = ThrowableRigidbody.mass;
    }

    public virtual void SetUp()
    {

    }

    public void SetLive()
    {
        TurnOffInteraction();
        SetCollidersToProjectile();
        if (explodeOnContact == false)
        {
            StartCoroutine(TimeUntilDetonation());
        }

        if (explodeOnContact == true)
        {
            EOC_armed = true;
        }

        Armed = true;
    }

    IEnumerator TimeUntilDetonation()
    {
        yield return new WaitForSeconds(ActivationTimer);
        Detonation();
    }

    public void Detonation()
    {
        Fire();
        StartCoroutine(KillSelf());
    }

    IEnumerator KillSelf()
    {
        waitingToDie = true;

        if (hasDespawnTimer == true)
        {
            yield return new WaitForSeconds(DespawnTimer);
        }

        Destroy(gameObject);
    }

    public override void Fire()
    {
        if (hasDespawnTimer == false)
        {
            Rigidbody TempRigidbody = GetComponent<Rigidbody>();
            TempRigidbody.useGravity = false;
            TempRigidbody.isKinematic = true;
        }

        foreach (GameObject i in projectileOrigins)
        {
            for (int x = 0; x < projectilesToFire; x++)
            {
                Rigidbody clone =
                     Instantiate(projectile,
                     i.transform.position,
                     i.transform.rotation);

                Projectile_Class projectileClone = clone.GetComponent<Projectile_Class>();

                projectileClone.ProjectileDamage = projectileClone.ProjectileDamage * DamageModifer;

                clone.transform.Rotate
     (Random.Range(-1 * projectileSpread, projectileSpread),
     Random.Range(-1 * projectileSpread, projectileSpread),
     0);

                clone.AddForce(clone.transform.forward * projectileSpeed);
            }
        }        
    }

    public void Release(int Force, Collider Thrower)
    {
        ThrowerToIgnore = Thrower;
        this.transform.parent = null;
        this.gameObject.AddComponent<Rigidbody>();
        Rigidbody TempRigidbody = GetComponent<Rigidbody>();
        TempRigidbody.mass = RigidBodyMassToStore;
        TempRigidbody.AddForce(transform.forward * Force);
        TempRigidbody.AddTorque(transform.right * ThrowSpin);
        TempRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public override void Activate(MasterInventory_Class inventory)
    {
        if (waitingToDie == false && inventory.readyToEquip == true)
        {
            TurnOffInteraction();
            inventory.EquipNewThrowable(this);
            MoveToPosition(inventory.ThrowablePosition);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != ThrowerToIgnore && Armed && waitingToDie == false)
        {
            if (ThrowableRigidbody != null && ThrowableRigidbody.mass != 0.01)
            {
                ThrowableRigidbody.mass = 0.01f;
            }

            if (EOC_armed == true)
            {
                Detonation();
                EOC_armed = false;
            }
        }   
    }
    #endregion

    void SetCollidersToProjectile()
    {
        foreach (Transform x in transform)
        {
            x.gameObject.layer = 11;
        }
    }
}
