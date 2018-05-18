using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleMaster : Unit_Master, IInteractable
{
    public Character_Master CurrentPilot_Character;
    public Equipment_Master CurrentPilot_Equipment;
    public Characters StartingPilot;
    public GameObject HumanPrefab;
    public Transform EjectPos;

    public override void Awake()
    {
        ShotAccMods[0] = QuickShotAccMod;
        ShotAccMods[1] = AimedShotAccMod;
        ShotAccMods[2] = SuppressShotAccMod;
        cantBeControlled = true;
        SetCharacter();
        SetItems();
        SetUpComponents();
        InstancePilot();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
        CalculateWeaponStats();
    }

    public void InstancePilot()
    {
        Character_Master initialPilot = (Character_Master)ScriptableObject.CreateInstance((StartingPilot).ToString());

        PilotEmbark(initialPilot, (Equipment_Master)ScriptableObject.CreateInstance((initialPilot.selectedEquipment).ToString()));

        CurrentPilot_Character.UnitStat_HitPoints = CurrentPilot_Character.UnitStat_StartingHitPoints;
        CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_StartingNerve;
    }

    public void PilotEmbark(Character_Master IncomingPilot, Equipment_Master IncomingPilotEquipment)
    {
        CurrentPilot_Character = Instantiate(IncomingPilot) as Character_Master;
        CurrentPilot_Equipment = Instantiate(IncomingPilotEquipment) as Equipment_Master;

        CurrentPilot_Character.UnitStat_HitPoints = IncomingPilot.UnitStat_HitPoints;
        CurrentPilot_Character.UnitStat_Nerve = IncomingPilot.UnitStat_Nerve;
        CurrentPilot_Equipment.Ammo = IncomingPilotEquipment.Ammo;

        characterSheet.UnitStat_FactionTag = CurrentPilot_Character.UnitStat_FactionTag;
        cantBeControlled = false;
        roundManager.AssignTeamColors(this);
        playerCamera.gameObject.SetActive(true);
    }

    public void PilotDisembark()
    {
        GameObject tempNewPilot = Instantiate(HumanPrefab, EjectPos.transform.position, EjectPos.transform.rotation);
        Unit_Human tempUnit_HumanScript = tempNewPilot.GetComponent<Unit_Human>();
        tempUnit_HumanScript.characterSheet = Instantiate(CurrentPilot_Character) as Character_Master;
        tempUnit_HumanScript.characterSheet.UnitStat_HitPoints = CurrentPilot_Character.UnitStat_HitPoints;
        tempUnit_HumanScript.characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;
        tempUnit_HumanScript.equippedEquipment = Instantiate(CurrentPilot_Equipment) as Equipment_Master;


        roundManager.SelectedUnit = tempUnit_HumanScript;
        roundManager.AssignTeamColors(tempUnit_HumanScript);
        CurrentPilot_Character = null;
  
        cantBeControlled = true;
        characterSheet.UnitStat_FactionTag = Character_Master.FactionTag.Neutral;
        roundManager.AssignTeamColors(this);
        ToggleControl(false);

        tempUnit_HumanScript.equippedEquipment.Ammo = CurrentPilot_Equipment.Ammo;
        CurrentPilot_Equipment = null;

        tempUnit_HumanScript.playerCamera.gameObject.SetActive(true);

    }

    public override void Die(string Attacker)
    {
        ChangeTeamNerve(-25);

        isDead = true;

        Destroy(KD_CC);

        foreach (Transform x in transform)
        {
            x.gameObject.AddComponent<Rigidbody>();
        }

        roundManager.AddNotificationToFeed(Attacker + " killed " + characterSheet.UnitStat_Name);
    }

    public void Activate(Unit_Human Activator)
    {
        if (CurrentPilot_Character == null)
        {
            Activator.cantBeControlled = true;
            PilotEmbark(Activator.characterSheet, Activator.equippedEquipment);
            Destroy(Activator.gameObject);
            roundManager.EndUnitTurn();
        }
    }

    public override void Interaction()
    {
        PilotDisembark();
        roundManager.EndUnitTurn();
    }

    public override void CalculateWeaponStats()
    {
        if (CurrentPilot_Character != null)
        {
            Calculated_WeaponAccuracy =
            (characterSheet.UnitStat_Accuracy + equippedWeapon.Accuracy + CurrentPilot_Character.UnitStat_Accuracy) / 3;

            if (CurrentPilot_Character.isPanicked)
            {
                Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * PanicAccMod;
            }
        }
    }

    public override void ChangeNerve(int change)
    {
        if (CurrentPilot_Character != null)
        {
            if (change > 0)
            {
                CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve + change;
            }

            if (change < 0 && RollStatCheck(characterSheet.UnitStat_Willpower, 1f) == false)
            {
                CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve + change;
            }

            #region Results from change
            if (CurrentPilot_Character.UnitStat_Nerve < 25 && !CurrentPilot_Character.isPanicked)
            {
                CurrentPilot_Character.isPanicked = true;
                roundManager.AddNotificationToFeed(CurrentPilot_Character.UnitStat_Name + " has Panicked!");
            }

            if (CurrentPilot_Character.UnitStat_Nerve > 25 && CurrentPilot_Character.isPanicked)
            {
                CurrentPilot_Character.isPanicked = false;
                roundManager.AddNotificationToFeed(CurrentPilot_Character.UnitStat_Name + " has Recovered!");
            }

            if (CurrentPilot_Character.UnitStat_Nerve > CurrentPilot_Character.UnitStat_StartingNerve)
            {
                CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_StartingNerve;
            }

            if (CurrentPilot_Character.UnitStat_Nerve < 0)
            {
                CurrentPilot_Character.UnitStat_Nerve = 0;
            }
        }
        #endregion
    }
}