using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Human : Unit_Master
{
    [Header("Human Fields")]

    public int initiativeRoll;
    bool initiativeRolled;
    public Unit_VehicleMaster PilottedVehicle;

    #region Unit Stats
    public int UnitStat_Level;
    public int UnitStat_ExperiencePoints;

    public enum PilotClass
    {
        Passion,
        Skillful,
        Tactical
    }

    public PilotClass SelectedPilotClass;

    //This is where knowledges will go at a later time >>>>

    public override void ToggleControl(bool toggle)
    {
        playerCamera.gameObject.SetActive(toggle);
        IsBeingControlled = toggle;
        SetItems();
        SetAction(0);
        CalculateCarryWeight();

        if (toggle == true && PilottedVehicle != null)
        {
            roundManager.SelectedUnit = PilottedVehicle;
            PilottedVehicle.ToggleControl(true);
            ToggleControl(false);
            Debug.Log("Turn on vehiclee");
        }
    }

    #endregion

    #region Combat Methods
    public override void Die(string Attacker)
    {
        //Debug.Log(this.gameObject.name + "has died");

        ChangeTeamNerve(-15);

        isDead = true;

        //if (roundManager.SelectedUnit == this)
        //{
        //    roundManager.EndUnitTurn();
        //}

        this.transform.localScale = new Vector3(1f, 0.25f, 1f);

        roundManager.AddNotificationToFeed(Attacker + " killed " + characterSheet.UnitStat_Name);
    }

    public void RollInitiative()
    {
        int tempCharacterReaction = characterSheet.UnitStat_Reaction;

        if (PilottedVehicle != null)
        {
            tempCharacterReaction = (tempCharacterReaction + PilottedVehicle.characterSheet.UnitStat_Reaction) * 2;
        }

        if (!initiativeRolled)
        {
            initiativeRoll = UnityEngine.Random.Range(0, 99);
            initiativeRolled = true;
        }

        characterSheet.UnitStat_Initiative = characterSheet.UnitStat_Initiative + initiativeRoll;
    }
    #endregion
}