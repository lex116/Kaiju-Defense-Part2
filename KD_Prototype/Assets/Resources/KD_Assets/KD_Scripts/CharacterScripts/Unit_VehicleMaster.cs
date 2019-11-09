using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_VehicleMaster : Unit_Master, IInteractable
{
    //temp 
    float CriticalFailure_Chance = 100;
    float CriticalFailure_EffectRadius = 30;
    //int CriticalFailure_Damage = 30;
    int CriticalFailure_Damage = 50;
    Item_Master.DamageTypes CriticalFailure_DamageType = Item_Master.DamageTypes.Explosive;


    [SerializeField]
    internal Character_Master CurrentPilot_Character;
    internal Equipment_Master CurrentPilot_Equipment;

    public Character_Master StartingPilot;
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

    public override void Setup(Character_Master infantry, Character_Master vehicle)
    {
        cantBeControlled = true;
        SetCharacter(vehicle);
        SetItems();
        SetUpComponents();
        InstancePilot();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
        CalculateWeaponStats();
        SetHardPoints();
        UnitIconName.text = characterSheet.UnitStat_Name;

        SetActions();

        Default_Reticle = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_Default"));
        manager_HUD.Reticle.sprite = Default_Reticle;
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
        KD_Global.AssignTeamColorsToUnit(this);
        playerCamera.gameObject.SetActive(true);
    }

    public void PilotDisembark()
    {
        cantBeControlled = true;
        characterSheet.UnitStat_FactionTag = KD_Global.FactionTag.Neutral;
        KD_Global.AssignTeamColorsToUnit(this);

        //change this to have the toggle control back but also not be fucked
        //ToggleControl(false);

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

        KD_Global.AssignTeamColorsToUnit(tempUnit_HumanScript);
        tempUnit_HumanScript.UnitIconName.text = tempUnit_HumanScript.characterSheet.UnitStat_Name;

        tempUnit_HumanScript.SetItems();
        tempUnit_HumanScript.equippedEquipment.Ammo = CurrentPilot_Equipment.Ammo;
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

        Sensor.DestroyHardPoint(Attacker);
        PrimaryWeapon.DestroyHardPoint(Attacker);
        SecondaryEquipment.DestroyHardPoint(Attacker);
        Locomotion.DestroyHardPoint(Attacker);

        foreach (Transform x in transform)
        {
            x.gameObject.AddComponent<Rigidbody>();
        }

        manager_HUD.AddNotificationToFeed(Attacker + " killed " + characterSheet.UnitStat_Name);

        CriticalFailureCheck();
    }

    public void Activate(Unit_Human Activator)
    {
        if (CurrentPilot_Character == null)
        {
            Activator.cantBeControlled = true;
            PilotEmbark(Activator.characterSheet, Activator.equippedEquipment);
            Destroy(Activator.gameObject);

            // change this to the gamestate manager later 
            //roundManager.EndUnitTurn();
        }
    }

    // reference the overlord for how completely fucked this is
    //public override void Interaction()
    //{
    //    PilotDisembark();
    //    roundManager.EndUnitTurn();
    //}

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
                manager_HUD.AddNotificationToFeed(CurrentPilot_Character.UnitStat_Name + " has Panicked!");
            }

            if (CurrentPilot_Character.UnitStat_Nerve > 25 && CurrentPilot_Character.isPanicked)
            {
                CurrentPilot_Character.isPanicked = false;
                characterSheet.isPanicked = false;
                manager_HUD.AddNotificationToFeed(CurrentPilot_Character.UnitStat_Name + " has Recovered!");
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

    public void CriticalFailureCheck()
    {
        int RollResult = 0;
        RollResult = UnityEngine.Random.Range(0, 99);

        if (RollResult < CriticalFailure_Chance)
        {
            CriticalFailure();
        }
    }

    public void CriticalFailure()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, CriticalFailure_EffectRadius / 2);

        foreach (Collider x in hitColliders)
        {
            //RaycastHit hit;

            //if (Physics.Raycast(transform.position, (x.transform.position - transform.position).normalized, out hit, CriticalFailure_EffectRadius))
            //{
                //if (hit.collider == x)
                //{ 
                    IDamagable objectToBeDamaged;

                    objectToBeDamaged = x.gameObject.GetComponent<IDamagable>();

                    if (objectToBeDamaged != null)
                    {
                        objectToBeDamaged.TakeDamage(CriticalFailure_Damage, Item_Master.DamageTypes.Explosive, characterSheet.UnitStat_Name);
                    }
                //}
            //}
        }

        Instantiate((Resources.Load<GameObject>("KD_Assets/KD_Prefabs/TempExplosion")), transform.position, transform.rotation);

        manager_HUD.AddNotificationToFeed(characterSheet.UnitStat_Name + " goes critical!");
    }
}