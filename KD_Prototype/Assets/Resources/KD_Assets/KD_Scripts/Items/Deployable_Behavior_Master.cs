using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployable_Behavior_Master : MonoBehaviour
{
    internal Unit_Master DeployableOwner;

    public GameObject DamageSphere;

    bool isArmed;

    void Awake()
    {
        Invoke("Arm", 1f);
    }

    void Arm()
    {
        isArmed = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != DeployableOwner.gameObject 
            && other.gameObject.transform.parent != DeployableOwner.gameObject
            && isArmed)
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
                //NOTICE
                //DeployableOwner.ToggleMovingState();
            }
        }

        Destroy(this.gameObject);
    }
}
