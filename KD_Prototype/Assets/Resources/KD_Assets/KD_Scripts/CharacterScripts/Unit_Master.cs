using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit_Master : MonoBehaviour, IDamagable
{
    #region components
    public Camera gunCamera;
    public GameObject HeldWeapon_GunPrefab;
    public GameObject HeldWeapon_GunTip;
    public GameObject HeldWeapon_Pos;
    internal AudioListener CameraAudioListener;
    internal bool cantBeControlled;
    public GameObject body;
    #endregion

    #region stats
    internal int StartingSuppressionCharges = 3;
    internal int RechargeSuppressionRate = 3;
    public int SuppressionCharges = 0;
    #endregion

    #region throwing & shooting
    [SerializeField]
    internal float CurrentShotAccuracyModifier = 0;
    internal Throwing throwing;
    internal int throwRange = 75;
    internal float QuickShotAccMod = 0.75f;
    internal float AimedShotAccMod = 1f;
    internal float SuppressShotAccMod = 0.40f;
    internal float PanicAccMod = 0.75f;
    #endregion

    #region prep for extraction
    internal Sprite Default_Reticle;
    #endregion

    #region actions
    [HideInInspector]
    public Action_Master[] Unit_Actions = new Action_Master[10];
    public Action_Master Selected_Unit_Action;
    internal int AP = 2;
    internal int Starting_Ap = 2;
    #endregion

    internal enum Unit_States
    {
        State_Waiting,
        State_Moving,
        State_PreparingToAct,
        State_Shooting,
        State_Dying,
        State_UsingEquipment,
        State_Embarking,
        State_Ejecting,
        State_ActivatingAbility
    }
    [SerializeField]
    internal Unit_States Current_Unit_State;

    internal enum Unit_Suppression_States
    {
        State_Waiting,
        State_WaitingToSuppress,
        State_PreparingToSuppress,
        State_Suppressing
    }
    [SerializeField]
    internal Unit_Suppression_States Current_Unit_Suppression_State;

    #region Unit Components
    public Character_Master characterSheet;
    [Header("Components")]
    //internal KD_CharacterController KD_CC;
    public Shooting shooting;
    public Camera playerCamera;
    //internal RoundManager roundManager;
    internal Manager_HUD manager_HUD;

    public Transform detectionNodes;
    public MeshRenderer[] UnitSkins;
    public Transform DeployableSpawnLocation;
    internal int DeployableThrowForce = 1500;


    #region map components
    public GameObject NonRotatingCanvas;
    public Text UnitIconName;
    internal Quaternion NonRotatingCanvasDefault = new Quaternion(0, 0, 0, 0);
    #endregion
    public Image MapIconHighlight;
    public Image UnitIcon;
    Quaternion UnitIconOrientation = new Quaternion(90, 0, 0, 0);
     #endregion
    
    #region Unit Stats
    [Header("Stats")]
    internal float startingMovementPoints;
    [HideInInspector]
    internal float movementPointsRemaining;
    internal float Calculated_WeaponAccuracy;
    internal bool hasNoMovementRemaining;
    internal bool isDead;
    #endregion
    
    #region Unit Input
    [Header("Input")]

    public bool IsBeingControlled;

    public GameObject aimingNode;
    internal Unit_Master suppressionTarget;



    internal float DefaultFOV = 60;
    internal float TargetFOV;
    internal bool isAbleToSuppress;
    internal bool isOnSuppressionCooldown;
    internal int SuppressionCooldownRate = 4;
    internal Vector3 movementPosition;
    #endregion

    #region Unit Inventory    
    [Header("Inventory")]
    public Weapon_Master equippedWeapon;
    public Equipment_Master equippedEquipment;
    public Armor_Master equippedArmor;
    #endregion

    //

    #region Set Up
    //run all the first time set up functions, reset or default things to new
    public virtual void Setup(KD_Global.Characters infantry, KD_Global.Characters vehicle)
    {
        SetUpComponents();
        SetCharacter(infantry);
        SetItems();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
        characterSheet.UnitStat_Nerve = characterSheet.UnitStat_StartingNerve;
        CalculateWeaponStats();
        //UnitIconName.text = characterSheet.UnitStat_Name;
        TargetFOV = DefaultFOV;
        Current_Unit_State = Unit_States.State_Waiting;
        Current_Unit_Suppression_State = Unit_Suppression_States.State_Waiting;
        SetActions();
        //notice
        //change this
        Default_Reticle = (Resources.Load<Sprite>("KD_Sprites/KD_Reticle_Default"));
        manager_HUD.Reticle.sprite = Default_Reticle;
        KD_Global.AssignTeamColorsToUnit(this);
    }
    //get references to components we need
    public void SetUpComponents()
    {
        aimingNode = GetComponentInChildren<AimingNode>().gameObject;
        shooting = GetComponent<Shooting>();
        shooting.unit = this;
        manager_HUD = FindObjectOfType<Manager_HUD>();
        CameraAudioListener = playerCamera.GetComponent<AudioListener>();
        CameraAudioListener.enabled = false;
        movementPosition = this.transform.position;
        throwing = GetComponent<Throwing>();
        throwing.unit = this;
        throwing.LaunchTransform = DeployableSpawnLocation.transform;
        throwing.lineRenderer = GetComponent<LineRenderer>();
        ResetMovement();
    }
    //reset movement for a new turn
    public void ResetMovement()
    {
        movementPointsRemaining = startingMovementPoints;
        hasNoMovementRemaining = false;
    }
    //instance the unit's character sheet
    public void SetCharacter(KD_Global.Characters selectedCharacter)
    { 
        characterSheet = (Character_Master)ScriptableObject.CreateInstance(selectedCharacter.ToString());
    }
    //instance the unit's inventory and get references to the heldweapon tip and model
    public virtual void SetItems()
    {
        if (equippedWeapon == null)
            equippedWeapon = (Weapon_Master)ScriptableObject.CreateInstance(characterSheet.selectedWeapon.ToString());

        HeldWeapon_GunPrefab = Instantiate(equippedWeapon.WeaponModel, HeldWeapon_Pos.transform);
        HeldWeapon_GunTip = HeldWeapon_GunPrefab.GetComponentInChildren<GunTip_Script>().gameObject;

        if (equippedEquipment == null)
            equippedEquipment = (Equipment_Master)ScriptableObject.CreateInstance(characterSheet.selectedEquipment.ToString());

        if (equippedArmor == null)
            equippedArmor = (Armor_Master)ScriptableObject.CreateInstance(characterSheet.selectedArmor.ToString());

        equippedEquipment.DeployableSpawnLocation = DeployableSpawnLocation;
        equippedEquipment.DeployableThrowForce = DeployableThrowForce;
        equippedEquipment.DeployableOwner = this;
    }
    //calculate the weapons accuracy
    public virtual void CalculateWeaponStats()
    {
        //
    }
    //instance the scriptable objects that hold the logic for unit actions
    public void SetActions()
    {
        Unit_Actions[1] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_Dash");
        Unit_Actions[2] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_QuickShot");
        Unit_Actions[3] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_AimedShot");
        Unit_Actions[4] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_SuppressShot");
        Unit_Actions[5] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_UseEquipment");
        Unit_Actions[6] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_Interaction");
        Unit_Actions[0] = null;
    }
    #endregion

    #region Clean Up, Resets, Calculations
    public void ResetAP()
    {
        AP = Starting_Ap;
    }
    
    public virtual void CalculateCarryWeight()
    {
        float CarryCapacity = characterSheet.UnitStat_Fitness / 4;
        float CarryWeight = equippedWeapon.Weight + equippedEquipment.Weight + equippedArmor.Weight;
        float CarryWeightDifference = CarryCapacity - CarryWeight;
        float Encumberance = CarryWeightDifference / CarryCapacity;

        startingMovementPoints = characterSheet.UnitStat_Fitness;
        movementPointsRemaining = startingMovementPoints * Encumberance;
    }
    #endregion

    #region HUD or UI

    public void ResetCameraFOV()
    {
        playerCamera.fieldOfView = DefaultFOV;
        gunCamera.fieldOfView = DefaultFOV;
    }
    public void ScaleCameraFOV()
    {
        #region storage
        TargetFOV = (int)(DefaultFOV - (DefaultFOV * ((Calculated_WeaponAccuracy * CurrentShotAccuracyModifier) / 100)));

        playerCamera.fieldOfView = TargetFOV;
        gunCamera.fieldOfView = TargetFOV;
        //playerCamera.fieldOfView = (int)(DefaultFOV - (DefaultFOV * (Calculated_WeaponAccuracy / 100)));

        //TargetFOV = DefaultFOV;
        //playerCamera.fieldOfView = DefaultFOV;

        //if (playerCamera.fieldOfView != TargetFOV)
        //{
        //    if (playerCamera.fieldOfView < TargetFOV)
        //        playerCamera.fieldOfView = playerCamera.fieldOfView + FieldOfViewChangeRate;

        //    if (playerCamera.fieldOfView > TargetFOV)
        //        playerCamera.fieldOfView = playerCamera.fieldOfView - FieldOfViewChangeRate;
        //}
        #endregion
    }

    //come back to this later it isnt very relevant during this period of great mourning
    //public void OrientUnitIcon()
    //{
    //    NonRotatingCanvas.transform.rotation = Quaternion.identity;
    //}

    #endregion

    #region Combat Methods
    public virtual void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        if (isDead == false)
        {
            int DamageToTake = 0;

            DamageToTake = Damage - (equippedArmor.DamageResistance[(int)DamageType]);

            if (DamageToTake > 0)
            {
                //Test
                if (characterSheet.isPanicked == false)
                    DamageToTake = DamageToTake / 2;

                if (DamageToTake < 1)
                    DamageToTake = 1;

                ChangeTeamNerve(-DamageToTake);
                characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_HitPoints - DamageToTake;
            }

            if (characterSheet.UnitStat_HitPoints <= 0)
            {
                characterSheet.UnitStat_HitPoints = 0;
                Die(Attacker);
            }
        }
    }

    public virtual void TrackSuppressTarget()
    {
        //Rotate the body to face the target
        transform.LookAt(suppressionTarget.transform);

        Vector3 bodyEulerAngles = transform.rotation.eulerAngles;
        bodyEulerAngles.x = 0;
        bodyEulerAngles.z = 0;

        transform.rotation = Quaternion.Euler(bodyEulerAngles);

        //Rotate the camera to face the target
        aimingNode.transform.LookAt(suppressionTarget.transform);

        Vector3 camEulerAngles = aimingNode.transform.rotation.eulerAngles;
        bodyEulerAngles.y = 0;
        bodyEulerAngles.z = 0;

        aimingNode.transform.rotation = Quaternion.Euler(camEulerAngles);
    }

    public virtual void SuppressionUpdate()
    {
        if (isDead == false && suppressionTarget!= null && suppressionTarget.isDead == false)
        {
            TrackSuppressTarget();

            bool losCheck = false;

            foreach (Transform x in suppressionTarget.detectionNodes)
            {
                if (losCheck == false)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(aimingNode.transform.position, x.position - aimingNode.transform.position, out hit, equippedWeapon.Range))
                    {
                        if (suppressionTarget == hit.collider.GetComponent<Unit_Master>())
                        {
                            losCheck = true;
                        }

                        Unit_VehicleHardPoint tempHardPoint = hit.collider.GetComponent<Unit_VehicleHardPoint>();

                        if (tempHardPoint != null)
                        {
                            losCheck = true;
                        }
                    }
                }
            }

            if (shooting.isFiring == false && losCheck == true && !isOnSuppressionCooldown && SuppressionCharges > 0)
            {
                isOnSuppressionCooldown = true;
                shooting.TestShooting(SuppressShotAccMod);
                StartCoroutine(SuppressionCooldownRoutine());
                SuppressionCharges = SuppressionCharges - 1;
            }
        }
    }
    
    public IEnumerator SuppressionCooldownRoutine()
    {
        yield return new WaitForSeconds(equippedWeapon.FireRate * SuppressionCooldownRate);
        isOnSuppressionCooldown = false;
    }

    public virtual void Die(string Attacker)
    {
        //
    }

    public virtual void ChangeNerve(int change)
    {
        //
    }

    public void ChangeTeamNerve(int change)
    {
        Unit_Master[] allUnitsToChange = FindObjectsOfType<Unit_Master>();

        foreach (Unit_Master x in allUnitsToChange)
        {
            if (x.characterSheet.UnitStat_FactionTag == characterSheet.UnitStat_FactionTag)
            {
                x.ChangeNerve(change);
            }
        }
    }

    public bool RollStatCheck(int Stat, float StatMod)
    {
        int Roll = UnityEngine.Random.Range(0, 99);

        if (Roll < (Stat * StatMod))
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public void RecoverNerve()
    {
        ChangeNerve(characterSheet.UnitStat_Willpower / 10);
        //characterSheet.UnitStat_Nerve = characterSheet.UnitStat_Nerve + (characterSheet.UnitStat_Willpower / 10);
    }

    public void RollInitiative()
    {
        if (characterSheet.initiativeRolled == false)
        {
            characterSheet.initiativeRoll = UnityEngine.Random.Range(0, 99);
            characterSheet.UnitStat_Initiative = characterSheet.UnitStat_Reaction + characterSheet.initiativeRoll;
            characterSheet.initiativeRolled = true;
        }
    }

    public void RechargeSuppression()
    {
        SuppressionCharges = SuppressionCharges + RechargeSuppressionRate;
    }

    #endregion
}