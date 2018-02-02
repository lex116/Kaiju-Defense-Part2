using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterFacility_Class : MonoBehaviour, IInteractable
{
    [HideInInspector]
    public string OK_Text;
    [HideInInspector]
    public string notOK_Text;
    public float Charge = 0;
    public Health Generator;
    [HideInInspector]
    public float ChargeRate;

    public string DisplayText
    {
        get
        {
            if (Charge == 100 && Generator.isDestroyed == false)
            {
                return OK_Text;
            }

            else
            {
                return notOK_Text;
            }
        }
    }

    public void Activate(MasterInventory_Class inventory)
    {
        FacilityEffect(inventory);
    }

    public virtual void FacilityEffect(MasterInventory_Class inventory)
    {

    }

    void Awake()
    {
        SetUp();
        StartCoroutine(ChargeRoutine());
    }

    public virtual void SetUp()
    {

    }

    IEnumerator ChargeRoutine()
    {
        yield return new WaitForSeconds(1f);

        if (Charge < 100 && Generator.isDestroyed == false)
        {
            Charge = (100 / ChargeRate) + Charge;

            if (Charge > 100)
            {
                Charge = 100;
            }
        }

        StartCoroutine(ChargeRoutine());
    }
}
