using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_FragGrenadePack : Equipment_Master
{
    public override void SetUp()
    {
        Ammo = 2;
        Projectile = (GameObject)(Resources.Load("KD_Assets/KD_Prefabs/FragGrenade"));
    }

    public override void UseEffect()
    {
        if (Ammo > 0)
        { 
            GameObject tempGrenadeGO =
                Instantiate(Projectile, DeployableSpawnLocation.transform.position, DeployableSpawnLocation.transform.rotation);

            Rigidbody tempGrenadeRB = tempGrenadeGO.GetComponent<Rigidbody>();

            FragGrenade_Behavior tempGrenadeScript = tempGrenadeGO.GetComponent<FragGrenade_Behavior>();
            tempGrenadeScript.Owner = DeployableOwner;

            tempGrenadeRB.AddForce(DeployableSpawnLocation.transform.forward * DeployableThrowForce);

            Ammo--;
        }
    }
}
