using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleMaster : Unit_Master
{
    public Unit_Human CurrentPilot;
    public Transform CockpitPos;
    public Transform EjectPos;

    public GameObject PilotPrefabGameObject;
    public Characters SelectedPilot;


    public override void Awake()
    {
        SetCharacter();
        SetUpComponents();
        InstancePilot();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
    }

    public void InstancePilot()
    {
        GameObject tempUnitGameObject = Instantiate(PilotPrefabGameObject, transform.position, transform.rotation);
        Unit_Human tempUnitHuman = tempUnitGameObject.GetComponent<Unit_Human>();
        tempUnitHuman.selectedCharacter = SelectedPilot;
        
        PilotEmbark(tempUnitHuman);
    }

    public void PilotEmbark(Unit_Human IncomingPilot)
    {
        CurrentPilot = IncomingPilot;
        CurrentPilot.PilottedVehicle = this;
        CurrentPilot.transform.position = CockpitPos.position;
        CurrentPilot.transform.parent = this.transform;
        CurrentPilot.MapIconCanvas.SetActive(false);
        characterSheet.UnitStat_FactionTag = CurrentPilot.characterSheet.UnitStat_FactionTag;
    }

    public void PilotDisembark(Unit_Human OutgoingPilot)
    {

    }

    public void CalculateCombinedStats()
    {
           
    }

    public override void Die(string Attacker)
    {
        ChangeTeamNerve(-25);

        //Debug.Log(this.gameObject.name + "has died");

        isDead = true;

        //if (roundManager.SelectedUnit == this)
        //{
        //    roundManager.EndUnitTurn();
        //}

        //this.transform.localScale = new Vector3(1f, 0.25f, 1f);
        //temp
        Destroy(KD_CC);

        foreach (Transform x in transform)
        {
            x.gameObject.AddComponent<Rigidbody>();
        }

        roundManager.AddNotificationToFeed(Attacker + " killed " + characterSheet.UnitStat_Name);
    }
}
