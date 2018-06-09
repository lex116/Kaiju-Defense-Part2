using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployable_Behavior_Master : MonoBehaviour
{
    internal Unit_Master DeployableOwner;

    public GameObject DamageSphere;

    void Awake()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != DeployableOwner.gameObject)
        {
            Detonate();
        }
    }

    public virtual void Detonate()
    {
        //
    }

    public void CleanUp()
    {
        if (DeployableOwner.isDead == false)
        {
            DeployableOwner.Current_Unit_State = Unit_Master.Unit_States.State_PreparingToAct;

            if (DeployableOwner.AP == 0)
            {
                DeployableOwner.Current_Unit_State = Unit_Master.Unit_States.State_Moving;
            }
        }

        Destroy(this.gameObject);
    }
}
