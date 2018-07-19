using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployable_Behavior_FragGrenadePack : Deployable_Behavior_Master
{
    public override void Detonate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DeployableOwner.equippedEquipment.EffectRadius);

        foreach (Collider x in hitColliders)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, (x.transform.position - transform.position).normalized, out hit, DeployableOwner.equippedEquipment.EffectRadius / 2))
            {
                if (hit.collider == x)
                {
                    IDamagable objectToBeDamaged;

                    objectToBeDamaged = x.gameObject.GetComponent<IDamagable>();

                    if (objectToBeDamaged != null)
                    {
                        objectToBeDamaged.TakeDamage(DeployableOwner.equippedEquipment.Damage, DeployableOwner.equippedEquipment.damageType, DeployableOwner.characterSheet.UnitStat_Name);
                    }
                }
            }
        }

        RoundManager RM = FindObjectOfType<RoundManager>();

        RM.AddNotificationToFeed("Frag Grenade Exploded");

        Instantiate(DamageSphere, transform.position, transform.rotation);

        CleanUp();
    }
}
