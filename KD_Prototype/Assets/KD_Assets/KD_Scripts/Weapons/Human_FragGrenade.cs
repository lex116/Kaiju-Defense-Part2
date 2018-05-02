using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_FragGrenade : MonoBehaviour
{
    public Unit_Master Owner;
    public GameObject ExplosionSphere;
    public int ExplosionRadius;
    int ExplosionDamage = 8;

    void Start()
    {
        Setfuse();
    }

    public void Setfuse()
    {
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

                        objectToBeDamaged.TakeDamage(damageToDeal, Owner.UnitStat_Name);
                    }
                }
            }

            Destroy(this.gameObject);
        }
    }
}
