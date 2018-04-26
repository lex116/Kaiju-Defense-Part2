using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Human : Unit_Master
{
    public Unit_VehicleMaster pilotedVehicle;

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

    #endregion

    #region Utility + Setup Methods
    public override void SetUp()
    {
        KD_CC = GetComponent<KD_CharacterController>();
        shooting = GetComponent<Shooting>();
        shooting.unit = this;
        ShootingStateMachine = GetComponent<Animator>();
        roundManager = FindObjectOfType<RoundManager>();

        movementPosition = this.transform.position;

        UnitStat_StartingHitPoints = 15;
        startingMovementPoints = 70;

        ResetMovement();
    }
    #endregion

    #region Combat Methods
    public override void TakeDamage(int Damage)
    {
        if (isDead == false)
        {
            UnitStat_HitPoints = UnitStat_HitPoints - Damage;
            if (UnitStat_HitPoints <= 0)
            {
                UnitStat_HitPoints = 0;
                Die();
            }
        }
    }

    public override void Die()
    {
        Debug.Log(this.gameObject.name + "has died");

        isDead = true;

        if (roundManager.SelectedUnit == this)
        {
            roundManager.EndUnitTurn();
        }

        this.transform.localScale = new Vector3(1f, 0.25f, 1f);
    }


    #endregion
}