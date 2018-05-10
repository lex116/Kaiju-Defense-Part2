using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_MechaMaster : Unit_VehicleMaster
{
    public Unit_VehicleHardPoint Sensor;
    public Unit_VehicleHardPoint RightArm;
    public Unit_VehicleHardPoint LeftArm;
    public Unit_VehicleHardPoint Legs;

    public override void SetUp()
    {
        KD_CC = GetComponent<KD_CharacterController>();
        shooting = GetComponent<Shooting>();
        shooting.unit = this;
        ShootingStateMachine = GetComponent<Animator>();
        roundManager = FindObjectOfType<RoundManager>();

        movementPosition = this.transform.position;

        startingMovementPoints = 50;

        ResetMovement();
    }

    //public override void TakeDamage(int Damage, string Attacker)
    //{
    //    if (isDead == false)
    //    {
    //        Damage = Damage - DamageResist;

    //        if (Damage > 0)
    //        {
    //            characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_HitPoints - Damage;
    //        }

    //        if (characterSheet.UnitStat_HitPoints <= 0)
    //        {
    //            characterSheet.UnitStat_HitPoints = 0;
    //            Die(Attacker);
    //        }
    //    }
    //}

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
