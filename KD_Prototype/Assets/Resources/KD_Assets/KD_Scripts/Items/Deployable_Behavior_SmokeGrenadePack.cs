using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployable_Behavior_SmokeGrenadePack : Deployable_Behavior_Master
{
    public GameObject SmokeCloud;

    public override void Detonate()
    {
        //NOTICE
        //RoundManager RM = FindObjectOfType<RoundManager>();
        //RM.AddNotificationToFeed("Smoke Grenade Exploded");

        GameObject tempSmokeCloud = Instantiate(SmokeCloud, transform.position, transform.rotation);

        tempSmokeCloud.transform.localScale = new Vector3(DeployableOwner.equippedEquipment.EffectRadius, DeployableOwner.equippedEquipment.EffectRadius, DeployableOwner.equippedEquipment.EffectRadius);

        CleanUp();
    }
}
