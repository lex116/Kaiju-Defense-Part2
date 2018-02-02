using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Napalm : Projectile_Class
{
    bool isMultiplying;
    float multiplyRate = 5f;
    float armTime = 0.75f;
    float multiplyForce = 1f;
    [SerializeField]
    GameObject Clone;
    [SerializeField]
    GameObject SpawnPos;

    void Update()
    {
        if (isAttachedDOT)
        {
            transform.localScale = new Vector3(
                transform.localScale.x + Random.Range(-0.01f, 0.01f),
                transform.localScale.y + Random.Range(-0.01f, 0.01f),
                transform.localScale.z + Random.Range(-0.01f, 0.01f));

            if (isMultiplying == false)
            {
                isMultiplying = true;
                StartCoroutine(Multiply());
            }
        }
    }

    IEnumerator Multiply()
    {
        yield return new WaitForSeconds(multiplyRate);

        GameObject fireClone =
            Instantiate(Clone, SpawnPos.transform.position, transform.rotation);

        fireClone.layer = 11;

        Napalm fireCloneScript = fireClone.GetComponent<Napalm>();
        fireCloneScript.ProjectileDamage = ProjectileDamage;
        fireCloneScript.isDud = true;
        fireCloneScript.isAttachedDOT = false;
        Rigidbody fireCloneRb = fireClone.GetComponent<Rigidbody>();
        fireCloneRb.isKinematic = false;
        fireCloneRb.useGravity = true;
            
        fireCloneRb.transform.rotation = new Quaternion(
                Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180), 0);
        fireCloneRb.AddForce(fireCloneRb.transform.forward * multiplyForce);

        yield return new WaitForSeconds(armTime);

        Collider fireCloneCol = fireClone.GetComponent<Collider>();
        fireCloneCol.isTrigger = false;

        StartCoroutine(Multiply());
    }

    public override void SetUp()
    {
        ProType = ProjectileType.DOT;
        DudTime = 2f;
        Lifetime = 1f;
        Bounces = 0;
        Ticks = 10;
    }
}
