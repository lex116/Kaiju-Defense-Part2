using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleMaster : Unit_Master, IInteractable
{
    [SerializeField]
    internal Character_Master CurrentPilot_Character;
    internal Equipment_Master CurrentPilot_Equipment;

    public Characters StartingPilot;
    public GameObject HumanPrefab;
    public Transform EjectPos;

    public Unit_VehicleHardPoint Sensor;
    public Unit_VehicleHardPoint PrimaryWeapon;
    public Unit_VehicleHardPoint SecondaryEquipment;
    public Unit_VehicleHardPoint Locomotion;

    public string ActivateText
    {
        get
        {
            return "Enter Vehicle";
        }
    }

    public override void Awake()
    {
        cantBeControlled = true;
        SetCharacter();
        SetItems();
        SetUpComponents();
        InstancePilot();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
        CalculateWeaponStats();
        SetHardPoints();
        UnitIconName.text = characterSheet.UnitStat_Name;

        SetActions();

        Default_Reticle = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_Default"));
        roundManager.Reticle.sprite = Default_Reticle;
    }

    public void InstancePilot()
    {
        Character_Master initialPilot = (Character_Master)ScriptableObject.CreateInstance((StartingPilot).ToString());

        PilotEmbark(initialPilot, (Equipment_Master)ScriptableObject.CreateInstance((initialPilot.selectedEquipment).ToString()));

        CurrentPilot_Character.UnitStat_HitPoints = CurrentPilot_Character.UnitStat_StartingHitPoints;
        CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_StartingNerve;

        characterSheet.UnitStat_StartingNerve = CurrentPilot_Character.UnitStat_StartingNerve;
        characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;

        RollStartingPilotInitiative();
    }

    public void PilotEmbark(Character_Master IncomingPilot, Equipment_Master IncomingPilotEquipment)
    {
        CurrentPilot_Character = Instantiate(IncomingPilot) as Character_Master;
        CurrentPilot_Equipment = Instantiate(IncomingPilotEquipment) as Equipment_Master;

        CurrentPilot_Character.UnitStat_HitPoints = IncomingPilot.UnitStat_HitPoints;
        CurrentPilot_Character.UnitStat_Nerve = IncomingPilot.UnitStat_Nerve;
        CurrentPilot_Equipment.Ammo = IncomingPilotEquipment.Ammo;

        characterSheet.UnitStat_FactionTag = CurrentPilot_Character.UnitStat_FactionTag;
        characterSheet.UnitStat_StartingNerve = CurrentPilot_Character.UnitStat_StartingNerve;
        characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;

        cantBeControlled = false;
        roundManager.AssignTeamColors(this);
        playerCamera.gameObject.SetActive(true);
    }

    public void PilotDisembark()
    {
        cantBeControlled = true;
        characterSheet.UnitStat_FactionTag = Character_Master.FactionTag.Neutral;
        roundManager.AssignTeamColors(this);
        ToggleControl(false);

        ///

        GameObject tempNewPilot = Instantiate(HumanPrefab, EjectPos.transform.position, EjectPos.transform.rotation);
        Unit_Human tempUnit_HumanScript = tempNewPilot.GetComponent<Unit_Human>();

        tempUnit_HumanScript.characterSheet = null;
        tempUnit_HumanScript.equippedArmor = null;
        tempUnit_HumanScript.equippedEquipment = null;
        tempUnit_HumanScript.equippedWeapon = null;

        tempUnit_HumanScript.characterSheet = Instantiate(CurrentPilot_Character) as Character_Master;

        tempUnit_HumanScript.characterSheet.UnitStat_HitPoints = CurrentPilot_Character.UnitStat_HitPoints;
        tempUnit_HumanScript.characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;

        tempUnit_HumanScript.characterSheet.initiativeRolled = true;
        tempUnit_HumanScript.characterSheet.UnitStat_Initiative = CurrentPilot_Character.UnitStat_Initiative;

        tempUnit_HumanScript.SetItems();
        tempUnit_HumanScript.equippedEquipment.Ammo = CurrentPilot_Equipment.Ammo;

        roundManager.AssignTeamColors(tempUnit_HumanScript);
        tempUnit_HumanScript.UnitIconName.text = tempUnit_HumanScript.characterSheet.UnitStat_Name;

        ///

        CalculateWeaponStats();
        CurrentPilot_Character = null;
        CurrentPilot_Equipment = null;
        UnitIconName.text = characterSheet.UnitStat_Name;
    }

    public override void Die(string Attacker)
    {
        ChangeTeamNerve(-25);

        if (CurrentPilot_Character != null)
        {
            if (RollStatCheck(CurrentPilot_Character.UnitStat_Fitness, 1) == true)
            {
                PilotDisembark();
            }
        }

        isDead = true;

        //Destroy(KD_CC);

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

    public override void ChangeNerve(int change)
    {
        if (CurrentPilot_Character != null)
        {
            if (change > 0)
            {
                CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve + change;
                characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;
            }

            if (change < 0 && RollStatCheck(characterSheet.UnitStat_Willpower, 1f) == false)
            {
                CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve + change;
                characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;
            }

            #region Results from change
            if (CurrentPilot_Character.UnitStat_Nerve < 25 && !CurrentPilot_Character.isPanicked)
            {
                CurrentPilot_Character.isPanicked = true;
                characterSheet.isPanicked = true;
                roundManager.AddNotificationToFeed(CurrentPilot_Character.UnitStat_Name + " has Panicked!");
            }

            if (CurrentPilot_Character.UnitStat_Nerve > 25 && CurrentPilot_Character.isPanicked)
            {
                CurrentPilot_Character.isPanicked = false;
                characterSheet.isPanicked = false;
                roundManager.AddNotificationToFeed(CurrentPilot_Character.UnitStat_Name + " has Recovered!");
            }

            if (CurrentPilot_Character.UnitStat_Nerve > CurrentPilot_Character.UnitStat_StartingNerve)
            {
                CurrentPilot_Character.UnitStat_Nerve = CurrentPilot_Character.UnitStat_StartingNerve;
                characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;
            }

            if (CurrentPilot_Character.UnitStat_Nerve < 0)
            {
                CurrentPilot_Character.UnitStat_Nerve = 0;
                characterSheet.UnitStat_Nerve = CurrentPilot_Character.UnitStat_Nerve;
            }
        }
        #endregion
    }

    public void SetHardPoints()
    {
        Sensor.StartingHitPoints = characterSheet.Sensor_StartingHitPoints;
        Sensor.HitPoints = Sensor.StartingHitPoints;
        if (Sensor.AttachedArmor == null)
            Sensor.AttachedArmor = (Armor_Master)ScriptableObject.CreateInstance(characterSheet.Sensor_Armor.ToString());

        PrimaryWeapon.StartingHitPoints = characterSheet.PrimaryWeapon_StartingHitPoints;
        PrimaryWeapon.HitPoints = PrimaryWeapon.StartingHitPoints;
        if (PrimaryWeapon.AttachedArmor == null)
            PrimaryWeapon.AttachedArmor = (Armor_Master)ScriptableObject.CreateInstance(characterSheet.PrimaryWeapon_Armor.ToString());

        SecondaryEquipment.StartingHitPoints = characterSheet.SecondaryEquipment_StartingHitPoints;
        SecondaryEquipment.HitPoints = SecondaryEquipment.StartingHitPoints;
        if (SecondaryEquipment.AttachedArmor == null)
            SecondaryEquipment.AttachedArmor = (Armor_Master)ScriptableObject.CreateInstance(characterSheet.SecondaryEquipment_Armor.ToString());

        Locomotion.StartingHitPoints = characterSheet.Locomotion_StartingHitPoints;
        Locomotion.HitPoints = Locomotion.StartingHitPoints;
        if (Locomotion.AttachedArmor == null)
            Locomotion.AttachedArmor = (Armor_Master)ScriptableObject.CreateInstance(characterSheet.Locomotion_Armor.ToString());
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

            if (PrimaryWeapon.isDestroyed)
            {
                Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * 0.75f;
            }

            if (Sensor.isDestroyed)
            {
                Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * 0.75f;
            }
        }
    }

    public override void CalculateCarryWeight()
    {
        float CarryCapacity = characterSheet.UnitStat_Fitness;
        float CarryWeight = equippedWeapon.Weight + equippedEquipment.Weight + equippedArmor.Weight;
        float CarryWeightDifference = CarryCapacity - CarryWeight;
        float Encumberance = CarryWeightDifference / CarryCapacity;

        startingMovementPoints = characterSheet.UnitStat_Fitness;
        movementPointsRemaining = startingMovementPoints * Encumberance;

        if (Locomotion.isDestroyed == true)
        {
            movementPointsRemaining = movementPointsRemaining / 4;
        }
    }

    public void RollStartingPilotInitiative()
    {
        if (CurrentPilot_Character.initiativeRolled == false)
        {
            CurrentPilot_Character.initiativeRoll = UnityEngine.Random.Range(0, 99);
            CurrentPilot_Character.initiativeRolled = true;
        }

        CurrentPilot_Character.UnitStat_Initiative = CurrentPilot_Character.UnitStat_Reaction + CurrentPilot_Character.initiativeRoll;
    }
}