using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGrenade_Behavior : MonoBehaviour
{
    public Unit_Master Owner;
    public string OwnerName;
    public GameObject ExplosionSphere;
    public int ExplosionRadius;
    int ExplosionDamage = 10;
    public Rigidbody grenadeRigidbody;
    public Vector3 grenadeVelocity;

    void Update()
    {
        grenadeVelocity = grenadeRigidbody.velocity;
    }

    void Start()
    {
        OwnerName = Owner.characterSheet.name;
        grenadeRigidbody = GetComponent<Rigidbody>();
        StartWaitToAddAction();
    }

    public void StartWaitToAddAction()
    {
        if(Owner != null)
        Owner.IsBeingControlled = false;

        StartCoroutine(WaitToAddAction());
    }

    IEnumerator WaitToAddAction()
    {
        yield return new WaitForSeconds(1f);

        int loops = 0;

        while (grenadeRigidbody.velocity != Vector3.zero)
        {
            yield return new WaitForFixedUpdate();
            loops = + loops + 1;

            if (loops > 100)
            {
                grenadeRigidbody.drag = 1;
                grenadeRigidbody.angularDrag = 1;
            }
        }

        RoundManager RM = FindObjectOfType<RoundManager>();

        Action_ActivateFragGrenade newActivateGrenade = new Action_ActivateFragGrenade();

        newActivateGrenade.SetUp(RM.SelectedUnit, RM.SelectedUnit.TargetUnit);

        newActivateGrenade.thisGrenade = this;

        RM.AddAction(newActivateGrenade);

        ExplosionSphere.SetActive(true);

        ExplosionSphere.transform.localScale = new Vector3(ExplosionRadius, ExplosionRadius, ExplosionRadius);
    }

    public void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

        foreach (Collider x in hitColliders)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, (x.transform.position - transform.position).normalized, out hit, Mathf.Infinity))
            {
                if (hit.collider == x)
                {
                    IDamagable objectToBeDamaged;

                    objectToBeDamaged = x.gameObject.GetComponent<IDamagable>();

                    if (objectToBeDamaged != null)
                    {
                        int damageToDeal = ExplosionDamage;

                        float dist = Vector3.Distance(x.transform.position, transform.position);

                        if (dist > ExplosionRadius / 2)
                        {
                            damageToDeal = ExplosionDamage / 2;
                        }

                        objectToBeDamaged.TakeDamage(damageToDeal, Item_Master.DamageTypes.Explosive, OwnerName);
                    }
                }
            }
        }

        RoundManager RM = FindObjectOfType<RoundManager>();

        RM.AddNotificationToFeed("Frag Grenade Exploded");

        Destroy(this.gameObject);
    }
}
