using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Master : MonoBehaviour, IDamagable
{
    #region Unit Component

    [Header("Components")]
    public KD_CharacterController KD_CC;
    public Shooting shooting;
    public Animator ShootingStateMachine;
    public Camera playerCamera;
    internal RoundManager roundManager;
    public Transform dectionNodes;

    #endregion

    #region Unit Stats
    public enum FactionTag
    {
        Echo,
        Shades,
        Russia
    }

    [Header("Stats")]

    [SerializeField]
    public FactionTag UnitStat_FactionTag;
    public string UnitStat_Name;
    public int UnitStat_Initiative;
    internal int UnitStat_StartingHitPoints;
    internal int UnitStat_HitPoints;
    public int UnitStat_StartingActionPoints;
    public int UnitStat_ActionPoints;
    public int UnitStat_Reaction;
    public int UnitStat_Accuracy;
    public int UnitStat_Willpower;
    public int UnitStat_Fitness;
    public float UnitStat_Speed;
    //public int UnitStat_Aptitude; ???
    public int UnitStat_VisualRange;
    public int UnitStat_StartingNerve;
    int UnitStat_Nerve;
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
    public enum DemoWeapon
    {
        Human_Pistol,
        Human_Shotgun,
        Human_MachineGun,
        Vehicle_MachineGun
    }
    
    [Header("Inventory")]

    [SerializeField]
    public DemoWeapon selectedWeapon;
    [SerializeField]
    public Weapon_Master currentWeapon;
    #endregion

    #region Utility + Setup Methods
    public void Awake()
    {
        SetUp();
        UnitStat_HitPoints = UnitStat_StartingHitPoints;
        UnitStat_Nerve = UnitStat_StartingNerve;
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
        if (selectedWeapon == DemoWeapon.Human_Pistol)
        {
            currentWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Human_Pistol");
        }

        if (selectedWeapon == DemoWeapon.Human_Shotgun)
        {
            currentWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Human_Shotgun");
        }

        if (selectedWeapon == DemoWeapon.Human_MachineGun)
        {
            currentWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Human_MachineGun");
        }

        if (selectedWeapon == DemoWeapon.Vehicle_MachineGun)
        {
            currentWeapon = (Weapon_Master)ScriptableObject.CreateInstance("Vehicle_MachineGun");
        }


        CalculateWeaponStats();
    }

    public void ToggleControl(bool toggle)
    {
        playerCamera.gameObject.SetActive(toggle);
        IsBeingControlled = toggle;
        SetWeapon();
        SetAction(0);
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
    }

    public void LookAtObject()
    {
        LookedAtObjectString = null;
        LookedAtUnit_Master = null;
        LookedAtUnit_VehicleHardPoint = null;

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, currentWeapon.Range))
        {
            if (hit.collider.gameObject.name != null)
            {
                LookedAtUnit_Master = hit.collider.gameObject.GetComponent<Unit_Master>();
                LookedAtUnit_VehicleHardPoint = hit.collider.gameObject.GetComponent<Unit_VehicleHardPoint>();

                if (LookedAtUnit_Master != null)
                    LookedAtObjectString = LookedAtUnit_Master.UnitStat_Name;

                if (LookedAtUnit_VehicleHardPoint != null)
                    LookedAtObjectString = LookedAtUnit_VehicleHardPoint.Name;
            }
        }
    }

    public void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = (UnitStat_Accuracy + currentWeapon.Accuracy) / 2;

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
    #endregion

    #region Combat Methods
    public virtual void TakeDamage(int Damage, string Attacker)
    {
        //
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

            // b.pos - a.pos

            //if (Physics.Raycast(AimingNode.transform.position, suppressionTarget.transform.position - AimingNode.transform.position, out hit, 100f))
            //{
            //    if (suppressionTarget == hit.collider.GetComponent<Unit_Master>())
            //    {
            //        losCheck = true;
            //    }

            //    Debug.DrawRay(AimingNode.transform.position, suppressionTarget.transform.position - AimingNode.transform.position, Color.red, 0.1f);
            //}

            foreach (Transform x in suppressionTarget.dectionNodes)
            {
                if (losCheck == false)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(AimingNode.transform.position, x.position - AimingNode.transform.position, out hit, 100f))
                    {
                        if (suppressionTarget == hit.collider.GetComponent<Unit_Master>())
                        {
                            losCheck = true;
                        }

                        //Debug.DrawRay(AimingNode.transform.position, x.position - AimingNode.transform.position, Color.red, 0.05f);
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
        UnitStat_Nerve = UnitStat_Nerve + change;

        if (UnitStat_Nerve < 50 && !isPanicked)
        {
            isPanicked = true;
            roundManager.AddNotificationToFeed(UnitStat_Name + " has Panicked!");
        }

        if (UnitStat_Nerve > 50 && isPanicked)
        {
            isPanicked = false;
            roundManager.AddNotificationToFeed(UnitStat_Name + " has Recovered!");
        }

        if (UnitStat_Nerve > 100)
        {
            UnitStat_Nerve = 100;
        }

        if (UnitStat_Nerve < 0)
        {
            UnitStat_Nerve = 0;
        }
    }

    public void ChangeTeamNerve(int change)
    {
        Unit_Master[] allUnitsToChange = FindObjectsOfType<Unit_Master>();

        foreach (Unit_Master x in allUnitsToChange)
        {
            if (x.UnitStat_FactionTag == UnitStat_FactionTag)
            {
                x.ChangeNerve(change);
            }
        }
    }
    #endregion
} 