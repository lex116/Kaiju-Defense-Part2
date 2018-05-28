using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Master : MonoBehaviour, IDamagable
{
    public GameObject NonRotatingCanvas;
    public Text UnitIconName;
    internal Quaternion NonRotatingCanvasDefault = new Quaternion(0, 0, 0, 0);

    internal float QuickShotAccMod = 0.75f;
    internal float AimedShotAccMod = 1f;
    internal float SuppressShotAccMod = 0.60f;
    
    internal float[] ShotAccMods = new float[3];

    internal float PanicAccMod = 0.75f;

    internal float CurrentShotAcc;

    internal int FieldOfViewChangeRate = 200;

    internal bool cantBeControlled;


    #region Unit Components
    public enum Characters
    {
        Character_SER_Samuel,
        Character_SER_Szymon,
        Character_SER_Emir,
        Character_SER_Kostas,
        Character_SER_Thomas,
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
    public Animator ShootingStateMachine;
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
    public enum Actions
    {
        QuickShot,
        AimedShot,
        SuppressShot
    }
    [Header("Input")]
    public Actions SelectedAction;

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

    #region Utility + Setup Methods
    public virtual void Awake()
    {
        ShotAccMods[0] = QuickShotAccMod;
        ShotAccMods[1] = AimedShotAccMod;
        ShotAccMods[2] = SuppressShotAccMod;
        SetUpComponents();
        SetCharacter();
        SetItems();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
        characterSheet.UnitStat_Nerve = characterSheet.UnitStat_StartingNerve;
        CalculateWeaponStats();
        UnitIconName.text = characterSheet.UnitStat_Name;
    }

    public void SetCharacter()
    {
        characterSheet = (Character_Master)ScriptableObject.CreateInstance((selectedCharacter).ToString());
    }

    public void SetUpComponents()
    {
        KD_CC = GetComponent<KD_CharacterController>();
        shooting = GetComponent<Shooting>();
        shooting.unit = this;
        ShootingStateMachine = GetComponent<Animator>();
        roundManager = FindObjectOfType<RoundManager>();
        movementPosition = this.transform.position;

        ResetMovement();
    }

    public void Start()
    {
        //this is for when unis get spawned in and need te references for some reason
        SetItems();
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

    public virtual void ToggleControl(bool toggle)
    {
        CalculateWeaponStats();
        playerCamera.gameObject.SetActive(toggle);
        IsBeingControlled = toggle;

        SetAction(0);
        CalculateCarryWeight();
    }

    public void FixedUpdate()
    {
        if (IsBeingControlled && isDead == false)
            KD_CC.InputUpdate();
    }

    public void Update()
    {
        if (IsBeingControlled && isDead == false)
        {
            PlayerInput();
            SpendMovement();
            LookAtTarget();
            ChangeCameraFOV();
        }

        OrientUnitIcon();
    }

    public virtual void PlayerInput()
    { 
        if (Input.GetKeyDown(KeyCode.Keypad1))
            SetAction(0);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            SetAction(1);

        if (Input.GetKeyDown(KeyCode.Keypad3))
            SetAction(2);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            bool ableToConfirm = true;

            if (SelectedAction == Actions.SuppressShot)
            {
                PaintTarget();

                if(suppressionTarget == null)
                {
                    ableToConfirm = false;
                }
            }

            if (ableToConfirm)
            {
                KD_CC.cantLook = true;
                ShootingStateMachine.SetInteger("ShootingMode", (int)SelectedAction + 1);
                roundManager.HUD_Player_ConfirmText.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && KD_CC.characterController.isGrounded)
        {
            KD_CC.cantMove = true;
            roundManager.HUD_Player_ConfirmText.SetActive(true);
            roundManager.EndUnitTurn();
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
            equippedEquipment.UseEffect();

        if (Input.GetKeyDown(KeyCode.M))
            roundManager.ToggleMiniMap();

        if (Input.GetKeyDown(KeyCode.E))
            Interaction();
    }

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

    public virtual void CalculateWeaponStats()
    {
        //
    }

    public void SetAction(int selection)
    {
        SelectedAction = (Actions)selection;

        CurrentShotAcc = ShotAccMods[selection];

        if (SelectedAction == Actions.QuickShot)
            TargetFOV = DefaultFOV;

        if (SelectedAction == Actions.AimedShot)
            TargetFOV = DefaultFOV - (DefaultFOV * (Calculated_WeaponAccuracy / 100));

        if (SelectedAction == Actions.SuppressShot)
            TargetFOV = (DefaultFOV - (DefaultFOV * (Calculated_WeaponAccuracy / 100)) / 2) ;
    }

    public void ChangeCameraFOV()
    {
        if (playerCamera.fieldOfView < TargetFOV)
            playerCamera.fieldOfView = playerCamera.fieldOfView + 0.01f * FieldOfViewChangeRate;

        if (playerCamera.fieldOfView > TargetFOV)
            playerCamera.fieldOfView = playerCamera.fieldOfView - 0.01f * FieldOfViewChangeRate;
    }

    public void OrientUnitIcon()
    {
        //NonRotatingCanvas.transform.LookAt(-1 * Vector3.forward);
        NonRotatingCanvas.transform.rotation = Quaternion.identity;

        //if (NonRotatingCanvas.transform.rotation != NonRotatingCanvasDefault)
        //NonRotatingCanvas.transform.rotation = NonRotatingCanvasDefault;
        //UnitIcon.transform.rotation = new Quaternion(45, 0, 0, 0);
        //UnitIcon.rectTransform.rotation = UnitIconOrientation;
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

    #region Combat Methods  
    public void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker)
    {
        if (isDead == false)
        {
            int DamageToTake = 0;

            DamageToTake = Damage - (equippedArmor.DamageResistance[(int)DamageType]);

            if (Damage > 0)
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

        if (LookedAtUnit_Master != null)
            suppressionTarget = LookedAtUnit_Master;

        if (LookedAtUnit_VehicleHardPoint != null)
            suppressionTarget = LookedAtUnit_VehicleHardPoint.GetComponentInParent<Unit_Master>();
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
        movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.x - movementPosition.x);
        movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.z - movementPosition.z);

        movementPosition = this.transform.position;

        if (movementPointsRemaining <= 0)
        {
            movementPointsRemaining = 0;
            if (KD_CC.characterController.isGrounded)
                KD_CC.cantMove = true;
        }
    }

    public void ResetMovement()
    {
        movementPointsRemaining = startingMovementPoints;
        hasNoMovementRemaining = false;
        KD_CC.cantMove = false;
        KD_CC.cantLook = false;
    }

    public void ResetStateMachine()
    {
        ShootingStateMachine.SetBool("isSPR", false);
        ShootingStateMachine.SetBool("Reset", true);
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

    public void ToggleSuppression(bool toggle)
    {
        isAbleToSuppress = toggle;
    }
    #endregion

    public virtual void Interaction()
    {
        //
    }
} 