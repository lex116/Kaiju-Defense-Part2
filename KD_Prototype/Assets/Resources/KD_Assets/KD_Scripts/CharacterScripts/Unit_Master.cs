using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Master : MonoBehaviour, IDamagable
{
    public MeshRenderer[] UnitSkins;

    public Image MapIconHighlight;

    public Image UnitIcon;
    Quaternion UnitIconOrientation = new Quaternion(0, -90, -90, 0);

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
        Character_GSR_Fedor,
        Character_GSR_Khabib,
        Character_GSR_Rustam
    }

    public Characters selectedCharacter;

    public Character_Master characterSheet;

    //temp
    public Equipment_Master equippedEquipment;

    public Transform DeployableSpawnLocation;
    //temp 1000
    int DeployableThrowForce = 1500;

    #region Unit Components

    [Header("Components")]
    public KD_CharacterController KD_CC;
    public Shooting shooting;
    public Animator ShootingStateMachine;
    public Camera playerCamera;
    internal RoundManager roundManager;
    public Transform dectionNodes;

    #endregion

    #region Unit Stats

    [Header("Stats")]
    internal float startingMovementPoints;
    [HideInInspector]
    internal float movementPointsRemaining;
    public float Calculated_WeaponAccuracy;
    public bool hasNoMovementRemaining;
    bool isPanicked;
    public bool isDead;
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
    [HideInInspector]
    public bool IsBeingControlled;
    //This doesnt do anything but it could if we change the way targetting works
    public Unit_Master TargetUnit;
    public GameObject AimingNode;
    public Unit_Master suppressionTarget;
    public string LookedAtObjectString;
    Unit_Master LookedAtUnit_Master;
    Unit_VehicleHardPoint LookedAtUnit_VehicleHardPoint;
    float DefaultFOV = 60;
    float TargetFOV;
    public bool isAbleToSuppress;
    public Vector3 movementPosition;
    #endregion

    #region Unit Inventory

    
    [Header("Inventory")]

    [SerializeField]
    public Weapon_Master equippedWeapon;

    public Armor_Master equippedArmor;
    #endregion

    #region Utility + Setup Methods
    public void Awake()
    {
        SetCharacter();
        SetUp();
        characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_StartingHitPoints;
        characterSheet.UnitStat_Nerve = characterSheet.UnitStat_StartingNerve;
    }

    public void SetCharacter()
    {
        characterSheet = (Character_Master)ScriptableObject.CreateInstance(((Characters)selectedCharacter).ToString());
    }

    public virtual void SetUp()
    {
        //
    }

    public void Start()
    {
        SetWeapon();
    }

    public void SetWeapon()
    {
        if (characterSheet.selectedWeapon == Character_Master.DemoWeapon.Human_Pistol)
        {
            equippedWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Human_Pistol");
        }

        if (characterSheet.selectedWeapon == Character_Master.DemoWeapon.Human_Shotgun)
        {
            equippedWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Human_Shotgun");
        }

        if (characterSheet.selectedWeapon == Character_Master.DemoWeapon.Human_MachineGun)
        {
            equippedWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Human_MachineGun");
        }

        if (characterSheet.selectedWeapon == Character_Master.DemoWeapon.Vehicle_MachineGun)
        {
            equippedWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Vehicle_MachineGun");
        }

        if (characterSheet.selectedWeapon == Character_Master.DemoWeapon.Human_AntiArmorRifle)
        {
            equippedWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Human_AntiArmorRifle");
        }

        equippedEquipment = (Equipment_Master)ScriptableObject.CreateInstance("Human_FragGrenadePack");
        equippedEquipment.DeployableSpawnLocation = DeployableSpawnLocation;
        equippedEquipment.DeployableThrowForce = DeployableThrowForce;
        equippedEquipment.DeployableOwner = this;

        equippedArmor = (Armor_Master)ScriptableObject.CreateInstance("Human_Uniform");

        CalculateWeaponStats();
    }

    public void ToggleControl(bool toggle)
    {
        playerCamera.gameObject.SetActive(toggle);
        IsBeingControlled = toggle;
        SetWeapon();
        SetAction(0);
        CalculateCarryWeight();
    }

    public virtual void Update()
    {
        if (IsBeingControlled && isDead == false)
        {
            PlayerInput();
            SpendMovement();
            LookAtObject();
            ChangeCameraFOV();
        }

        OrientUnitIcon();
    }

    public virtual void PlayerInput()
    {
        KD_CC.InputUpdate();

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
        {
            equippedEquipment.UseEffect();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            roundManager.ToggleMiniMap();
        }
    }

    public void LookAtObject()
    {
        LookedAtObjectString = null;
        LookedAtUnit_Master = null;
        LookedAtUnit_VehicleHardPoint = null;

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, equippedWeapon.Range))
        {
            if (hit.collider.gameObject.name != null)
            {
                LookedAtUnit_Master = hit.collider.gameObject.GetComponent<Unit_Master>();
                LookedAtUnit_VehicleHardPoint = hit.collider.gameObject.GetComponent<Unit_VehicleHardPoint>();

                if (LookedAtUnit_Master != null)
                    LookedAtObjectString = LookedAtUnit_Master.characterSheet.UnitStat_Name;

                if (LookedAtUnit_VehicleHardPoint != null)
                    LookedAtObjectString = LookedAtUnit_VehicleHardPoint.Name;
            }
        }
    }

    public void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = (characterSheet.UnitStat_Accuracy + equippedWeapon.Accuracy) / 2;

        if (isPanicked)
        {
            Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * 0.75f;
        }
    }

    public void SetAction(int selection)
    {
        SelectedAction = (Actions)selection;

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
            playerCamera.fieldOfView++;

        if (playerCamera.fieldOfView > TargetFOV)
            playerCamera.fieldOfView--;
    }

    public void OrientUnitIcon()
    {
            UnitIcon.rectTransform.rotation = UnitIconOrientation;
    }
    #endregion

    #region Combat Methods
    public virtual void TakeDamage(int Damage, string Attacker)
    {
        if (isDead == false)
        {
            ChangeTeamNerve(-Damage);

            characterSheet.UnitStat_HitPoints = characterSheet.UnitStat_HitPoints - Damage;
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
    }

    public void TrackSuppressTarget()
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

    public void SuppressionUpdate()
    {
        if (isAbleToSuppress == true && isDead == false)
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
                    }
                }
            }

            if (shooting.isFiring == false && losCheck == true)
            {
                shooting.TestShooting(.9f);
            }
        }
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

    public virtual void Die(string Attacker)
    {

    }

    public void ChangeNerve(int change)
    {
        if (change > 0)
        {
            characterSheet.UnitStat_Nerve = characterSheet.UnitStat_Nerve + change;
        }

        if (change < 0 && RollStat(characterSheet.UnitStat_Willpower, 1) == false)
        {
            characterSheet.UnitStat_Nerve = characterSheet.UnitStat_Nerve + change;
        }

        #region Results from change
        if (characterSheet.UnitStat_Nerve < 25 && !isPanicked)
        {
            isPanicked = true;
            roundManager.AddNotificationToFeed(characterSheet.UnitStat_Name + " has Panicked!");
        }

        if (characterSheet.UnitStat_Nerve > 25 && isPanicked)
        {
            isPanicked = false;
            roundManager.AddNotificationToFeed(characterSheet.UnitStat_Name + " has Recovered!");
        }

        if (characterSheet.UnitStat_Nerve > 100)
        {
            characterSheet.UnitStat_Nerve = 100;
        }

        if (characterSheet.UnitStat_Nerve < 0)
        {
            characterSheet.UnitStat_Nerve = 0;
        }
        #endregion
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

    public bool RollStat(int Stat, int StatMod)
    {
        int Roll = UnityEngine.Random.Range(0, 99);

        if (Roll > (Stat * StatMod))
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public void CalculateCarryWeight()
    {
        float CarryCapacity = characterSheet.UnitStat_Fitness / 4;
        float CarryWeight = equippedWeapon.Weight + equippedEquipment.Weight + equippedArmor.Weight;
        float CarryWeightDifference = CarryCapacity - CarryWeight;
        float Encumberance = CarryWeightDifference / CarryCapacity;

        startingMovementPoints = characterSheet.UnitStat_Fitness;
        movementPointsRemaining = startingMovementPoints * Encumberance;
    }
    #endregion
} 