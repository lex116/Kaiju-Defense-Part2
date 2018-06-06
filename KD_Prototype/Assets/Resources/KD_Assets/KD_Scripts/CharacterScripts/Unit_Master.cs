using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit_Master : MonoBehaviour, IDamagable
{
    internal float CurrentShotAccuracyModifier = 0;

    public Action_Master[] Unit_Actions = new Action_Master[10];
    public Action_Master Selected_Unit_Action;

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
        State_Suppressing
    }

    internal Unit_Suppression_States Current_Unit_Suppression_State;

    internal int AP = 2;
    internal int Starting_Ap = 2;

    AudioListener CameraAudioListener;

    public GameObject NonRotatingCanvas;
    public Text UnitIconName;
    internal Quaternion NonRotatingCanvasDefault = new Quaternion(0, 0, 0, 0);

    internal float QuickShotAccMod = 0.75f;
    internal float AimedShotAccMod = 1f;
    internal float SuppressShotAccMod = 0.60f;

    internal float PanicAccMod = 0.75f;

    internal bool cantBeControlled;

    #region Unit Components
    public enum Characters
    {
        Character_SER_Samuel,
        Character_SER_Szymon,
        Character_SER_Emir,
        Character_SER_Kostas,
        Character_SER_Thomas,
        Character_SER_Mecha_Xiphos,
        Character_SER_Mecha_Rogatina,
        Character_SCRAPS_Wanderlei,
        Character_SCRAPS_Anderson,
        Character_SCRAPS_Arlo,
        Character_SCRAPS_Mason,
        Character_SCRAPS_Noah,
        Character_SCRAPS_Mecha_Flyboy,
        Character_SCRAPS_Vehicle_Cobra,
        Character_GSR_Fedor,
        Character_GSR_Khabib,
        Character_GSR_Rustam
    }
    public Characters selectedCharacter;
    public Character_Master characterSheet;
    [Header("Components")]
    internal KD_CharacterController KD_CC;
    public Shooting shooting;
    public Camera playerCamera;
    internal RoundManager roundManager;
    public Transform dectionNodes;
    public MeshRenderer[] UnitSkins;
    public Image MapIconHighlight;
    public Image UnitIcon;
    Quaternion UnitIconOrientation = new Quaternion(90, 0, 0, 0);
    public Transform DeployableSpawnLocation;
    internal int DeployableThrowForce = 1500;
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

    internal bool IsBeingControlled;
    //This doesnt do anything but it could if we change the way targetting works
    internal Unit_Master TargetUnit;
    public GameObject AimingNode;
    internal Unit_Master suppressionTarget;

    internal Unit_Master LookedAtUnit_Master;
    internal Unit_VehicleHardPoint LookedAtUnit_VehicleHardPoint;

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
    public virtual void Awake()
    {
        SetUpComponents();
        SetCharacter();
        SetItems();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
        characterSheet.UnitStat_Nerve = characterSheet.UnitStat_StartingNerve;
        CalculateWeaponStats();
        UnitIconName.text = characterSheet.UnitStat_Name;
        TargetFOV = DefaultFOV;

        Current_Unit_State = Unit_States.State_Waiting;
        Current_Unit_Suppression_State = Unit_Suppression_States.State_Waiting;

        SetActions();
    }

    public void SetCharacter()
    {
        characterSheet = (Character_Master)ScriptableObject.CreateInstance((selectedCharacter).ToString());
    }
    public virtual void SetItems()
    {
        if (equippedWeapon == null)
            equippedWeapon = (Weapon_Master)ScriptableObject.CreateInstance(characterSheet.selectedWeapon.ToString());

        if (equippedEquipment == null)
            equippedEquipment = (Equipment_Master)ScriptableObject.CreateInstance(characterSheet.selectedEquipment.ToString());

        if (equippedArmor == null)
            equippedArmor = (Armor_Master)ScriptableObject.CreateInstance(characterSheet.selectedArmor.ToString());

        equippedEquipment.DeployableSpawnLocation = DeployableSpawnLocation;
        equippedEquipment.DeployableThrowForce = DeployableThrowForce;
        equippedEquipment.DeployableOwner = this;
    }
    public void SetUpComponents()
    {
        KD_CC = GetComponent<KD_CharacterController>();
        shooting = GetComponent<Shooting>();
        shooting.unit = this;
        roundManager = FindObjectOfType<RoundManager>();
        CameraAudioListener = playerCamera.GetComponent<AudioListener>();
        movementPosition = this.transform.position;
        ResetMovement();
    }

    public void SetActions()
    {
        Unit_Actions[1] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_Dash");
        Unit_Actions[2] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_QuickShot");
        Unit_Actions[3] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_AimedShot");
        Unit_Actions[4] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_SuppressShot");
        Unit_Actions[5] = (Action_Master)ScriptableObject.CreateInstance("Action_Basic_UseEquipment");
        Unit_Actions[0] = null;
    }
    #endregion

    #region Clean Up, Resets, Calculations
    public void ResetAP()
    {
        AP = Starting_Ap;
    }
    public virtual void CalculateWeaponStats()
    {
        //
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

    #region Updates
    public void FixedUpdate()
    {
        //ChangeCameraFOV();

        KD_CC.GroundCheckUpdate();

        if (Current_Unit_State != Unit_States.State_Dying)
            SpendMovement();

        if (Current_Unit_State == Unit_States.State_Moving &&
            movementPointsRemaining > 0)
            KD_CC.MovementUpdate();

        if (Current_Unit_State == Unit_States.State_Moving ||
            Current_Unit_State == Unit_States.State_PreparingToAct)
            KD_CC.LookUpdate();
    }

    public void Update()
    {
        if (IsBeingControlled && !isDead)
        {
            PlayerInput();
            LookAtTarget();
        }

        OrientUnitIcon();
    }
    #endregion

    #region HUD or UI
    public virtual void LookAtTarget()
    {
        LookedAtUnit_Master = null;
        LookedAtUnit_VehicleHardPoint = null;

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, equippedWeapon.Range))
        {
            if (hit.collider.gameObject.name != null)
            {
                LookedAtUnit_Master = hit.collider.gameObject.GetComponent<Unit_Master>();
                LookedAtUnit_VehicleHardPoint = hit.collider.gameObject.GetComponent<Unit_VehicleHardPoint>();
            }
        }
    }
    //Currently does nothing
    public void ChangeCameraFOV()
    {
        #region storage
        //TargetFOV = (int)(DefaultFOV - (DefaultFOV* (Calculated_WeaponAccuracy / 100)));
        ////playerCamera.fieldOfView = (int)(DefaultFOV - (DefaultFOV * (Calculated_WeaponAccuracy / 100)));

        //TargetFOV = DefaultFOV;
        ////playerCamera.fieldOfView = DefaultFOV;
        #endregion

        //if (playerCamera.fieldOfView != TargetFOV)
        //{
        //    if (playerCamera.fieldOfView < TargetFOV)
        //        playerCamera.fieldOfView = playerCamera.fieldOfView + FieldOfViewChangeRate;

        //    if (playerCamera.fieldOfView > TargetFOV)
        //        playerCamera.fieldOfView = playerCamera.fieldOfView - FieldOfViewChangeRate;
        //}
    }
    public void OrientUnitIcon()
    {
        NonRotatingCanvas.transform.rotation = Quaternion.identity;
    }
    #endregion

    #region Player Input
    public void ToggleControl(bool toggle)
    {
        playerCamera.gameObject.SetActive(toggle);
        CameraAudioListener.enabled = toggle;
        IsBeingControlled = toggle;

        if (toggle == true)
        {
            Current_Unit_State = Unit_States.State_Moving;
            Current_Unit_Suppression_State = Unit_Suppression_States.State_Waiting;
            Selected_Unit_Action = Unit_Actions[1];
            ResetAP();
            CalculateWeaponStats();
            CalculateCarryWeight();
        }

        if (toggle == false)
            Current_Unit_State = Unit_States.State_Waiting;
    }
    public virtual void PlayerInput()
    {
        //Switch between moving and firing mode
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Current_Unit_State == Unit_States.State_Moving)
            {
                Current_Unit_State = Unit_States.State_PreparingToAct;
                ChangeAction(1);
            }

            else if (Current_Unit_State == Unit_States.State_PreparingToAct)
            {
                Current_Unit_State = Unit_States.State_Moving;
                ChangeAction(1);
            }
        }

        if (Current_Unit_State == Unit_States.State_PreparingToAct)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                UseSelectedAction();

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                ChangeAction(1);

            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                ChangeAction(2);

            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                ChangeAction(3);

            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                ChangeAction(4);

            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                ChangeAction(5);
        }

        if (Input.GetKeyDown(KeyCode.B) && KD_CC.characterController.isGrounded && Current_Unit_State == Unit_States.State_Moving)
        {
            roundManager.HUD_Player_ConfirmText.SetActive(true);
            roundManager.EndUnitTurn();
        }

        if (Input.GetKeyDown(KeyCode.M))
            roundManager.ToggleMiniMap();

        //if (Input.GetKeyDown(KeyCode.E))
        //    Interaction();
    }
    public virtual void Interaction()
    {
        //
    }

    public void UseSelectedAction()
    {
        if (AP >= Selected_Unit_Action.Action_AP_Cost && Selected_Unit_Action.CheckRequirements(this) == true)
        {
            AP = AP - Selected_Unit_Action.Action_AP_Cost;
            Selected_Unit_Action.Action_Effect(this);
        }
        else
            roundManager.AddNotificationToFeed("Can't use that action!");
    }
    public void ChangeAction(int Selection)
    {
        if (Selected_Unit_Action != null)
            Selected_Unit_Action.Deselection_Effect(this);

        Selected_Unit_Action = Unit_Actions[Selection];
        Selected_Unit_Action.Selection_Effect(this);
    }
    #endregion

    #region Combat Methods  
    public void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        if (isDead == false)
        {
            int DamageToTake = 0;

            DamageToTake = Damage - (equippedArmor.DamageResistance[(int)DamageType]);

            if (DamageToTake > 0)
            {
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

    //Select the target to suppress
    public void PaintTarget()
    {
        suppressionTarget = null;

        Unit_Master possibleSuppressionTarget = null;

        if (LookedAtUnit_Master != null)
            possibleSuppressionTarget = LookedAtUnit_Master;

        if (LookedAtUnit_VehicleHardPoint != null)
            possibleSuppressionTarget = LookedAtUnit_VehicleHardPoint.GetComponentInParent<Unit_Master>();

        if (possibleSuppressionTarget != null)
        {
            if (possibleSuppressionTarget.characterSheet.UnitStat_Initiative < characterSheet.UnitStat_Initiative)
                suppressionTarget = possibleSuppressionTarget;

            else
                roundManager.AddNotificationToFeed("Can't suppress " + possibleSuppressionTarget.characterSheet.UnitStat_Name);
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
        AimingNode.transform.LookAt(suppressionTarget.transform);

        Vector3 camEulerAngles = AimingNode.transform.rotation.eulerAngles;
        bodyEulerAngles.y = 0;
        bodyEulerAngles.z = 0;

        AimingNode.transform.rotation = Quaternion.Euler(camEulerAngles);
    }

    public virtual void SuppressionUpdate()
    {
        if (isAbleToSuppress == true && isDead == false && suppressionTarget!= null)
        {
            TrackSuppressTarget();

            bool losCheck = false;

            foreach (Transform x in suppressionTarget.dectionNodes)
            {
                if (losCheck == false)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(AimingNode.transform.position, x.position - AimingNode.transform.position, out hit, equippedWeapon.Range))
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

            if (shooting.isFiring == false && losCheck == true && !isOnSuppressionCooldown)
            {
                isOnSuppressionCooldown = true;
                shooting.TestShooting(SuppressShotAccMod);
                StartCoroutine(SuppressionCooldownRoutine());
            }
        }
    }

    public IEnumerator SuppressionCooldownRoutine()
    {
        yield return new WaitForSeconds(equippedWeapon.FireRate * SuppressionCooldownRate);
        isOnSuppressionCooldown = false;
    }

    public void SpendMovement()
    {
        if (KD_CC.characterController.isGrounded)
        {
            movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.x - movementPosition.x);
            movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.z - movementPosition.z);
        }

        movementPosition = this.transform.position;

        if (movementPointsRemaining <= 0)
        {
            movementPointsRemaining = 0;
        }
    }

    public void ResetMovement()
    {
        movementPointsRemaining = startingMovementPoints;
        hasNoMovementRemaining = false;
    }

    public virtual void Die(string Attacker)
    {

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
    #endregion
} 