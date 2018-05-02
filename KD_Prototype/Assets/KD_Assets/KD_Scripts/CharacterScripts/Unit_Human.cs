using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Human : Unit_Master
{
    public Unit_VehicleMaster pilotedVehicle;

    public Rigidbody TestGrenade;
    public Transform GrenadeSpawnLocation;
    public int GrenadeThrowForce;

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
    public override void TakeDamage(int Damage, string Attacker)
    {
        if (isDead == false)
        {
            UnitStat_HitPoints = UnitStat_HitPoints - Damage;
            if (UnitStat_HitPoints <= 0)
            {
                UnitStat_HitPoints = 0;
                Die(Attacker);
            }
        }
    }

    public override void Die(string Attacker)
    {
        //Debug.Log(this.gameObject.name + "has died");

        isDead = true;

        if (roundManager.SelectedUnit == this)
        {
            roundManager.EndUnitTurn();
        }

        this.transform.localScale = new Vector3(1f, 0.25f, 1f);

        roundManager.AddNotificationToFeed(Attacker + " killed " + UnitStat_Name);
    }

    public override void PlayerInput()
    {
        #region Change Shot type
        KD_CC.InputUpdate();

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetAction_QuickShot();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetAction_AimedShot();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetAction_SuppressShot();
        }


        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (SelectedAction == Actions.QuickShot)
            {
                ShootingStateMachine.SetInteger("ShootingMode", 1);
                roundManager.HUD_Player_ConfirmText.SetActive(true);
            }

            if (SelectedAction == Actions.AimedShot)
            {
                ShootingStateMachine.SetInteger("ShootingMode", 2);
                roundManager.HUD_Player_ConfirmText.SetActive(true);
            }

            if (SelectedAction == Actions.SuppressShot)
            {
                PaintTarget();

                if (suppressionTarget != null)
                {
                    ShootingStateMachine.SetInteger("ShootingMode", 3);
                    roundManager.HUD_Player_ConfirmText.SetActive(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && KD_CC.characterController.isGrounded)
        {
            roundManager.HUD_Player_ConfirmText.SetActive(true);
            roundManager.EndUnitTurn();
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Rigidbody tempGrenade = 
                Instantiate(TestGrenade, GrenadeSpawnLocation.transform.position, GrenadeSpawnLocation.transform.rotation);
            Human_FragGrenade tempGrenadeScript = tempGrenade.GetComponent<Human_FragGrenade>();
            tempGrenadeScript.Owner = this;

            tempGrenade.AddForce(AimingNode.transform.forward * GrenadeThrowForce);
        }
    }
    #endregion


}