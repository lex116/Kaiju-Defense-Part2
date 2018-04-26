﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Master : MonoBehaviour, IDamagable
{
    public string FactionTag;

    public enum DemoWeapon
    {
        Human_Pistol,
        Human_Shotgun,
        Human_MachineGun,
        Vehicle_MachineGun
    }

    [SerializeField]
    public DemoWeapon selectedWeapon;

    #region Component Fields
    public KD_CharacterController KD_CC;
    [HideInInspector]
    public Shooting shooting;
    public bool IsBeingControlled;
    public Animator ShootingStateMachine;
    //This doesnt do anything but it could if we change the way targetting works
    public Unit_Master TargetUnit;
    public GameObject AimingNode;
    public GameObject Camera;
    internal RoundManager roundManager;
    [SerializeField]
    public Weapon currentWeapon;
    public Unit_Master suppressionTarget;
    public string LookedAtObject;
    #endregion

    #region Unit Stats
    public int UnitStat_Initiative;
    //[HideInInspector]
    internal int UnitStat_StartingHitPoints;
    internal int UnitStat_HitPoints;
    public int UnitStat_StartingActionPoints;
    public int UnitStat_ActionPoints;
    public int UnitStat_Reaction;
    //Temp Value
    public float UnitStat_Accuracy = .90f;
    //
    public int UnitStat_Willpower;
    public int UnitStat_Fitness;
    public float UnitStat_Speed;
    //public int UnitStat_Aptitude; ???
    public int UnitStat_VisualRange;
    public int UnitStat_Nerve;

    //

    public bool isDead;
    public Vector3 movementPosition;
    [HideInInspector]
    internal float startingMovementPoints;
    //internal float startingMovementPoints = 75;
    [HideInInspector]
    internal float movementPointsRemaining;
    public bool hasNoMovementRemaining;
    #endregion

    #region Calculated Weapon Stats
    //public int Calculated_WeaponDamage;
    public float Calculated_WeaponAccuracy;
    #endregion

    #region Utility + Setup Methods
    public void Awake()
    {
        SetUp();
        UnitStat_HitPoints = UnitStat_StartingHitPoints;
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
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Pistol");
        }

        if (selectedWeapon == DemoWeapon.Human_Shotgun)
        {
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Shotgun");
        }

        if (selectedWeapon == DemoWeapon.Human_MachineGun)
        {
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_MachineGun");
        }

        if (selectedWeapon == DemoWeapon.Vehicle_MachineGun)
        {
            currentWeapon = (Weapon)ScriptableObject.CreateInstance("Vehicle_MachineGun");
        }


        CalculateWeaponStats();
    }

    public void ToggleControl(bool toggle)
    {
        //KD_CC.AimingNode.SetActive(toggle);
        Camera.SetActive(toggle);
        IsBeingControlled = toggle;
        SetWeapon();
    }

    public void Update()
    {
        if (IsBeingControlled)
        {
            PlayerInput();
            SpendMovement();
            LookAtObject();
        }
    }

    public void PlayerInput()
    {
        KD_CC.InputUpdate();

        if (Input.GetKeyDown(KeyCode.U))
        {
            ShootingStateMachine.SetInteger("ShootingMode", 0);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShootingStateMachine.SetInteger("ShootingMode", 1);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ShootingStateMachine.SetInteger("ShootingMode", 2);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PaintTarget();

            if (suppressionTarget != null)
            {
                ShootingStateMachine.SetInteger("ShootingMode", 3);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            roundManager.EndUnitTurn();
        }
    }

    public void LookAtObject()
    {
        LookedAtObject = null;

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, currentWeapon.Range))
        {
            if (hit.collider.gameObject.name != null)
            {
                LookedAtObject = hit.collider.gameObject.name;
            }
        }
    }

    public void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = (UnitStat_Accuracy + currentWeapon.Accuracy) / 2;
    }
    #endregion

    #region Combat Methods
    public virtual void TakeDamage(int Damage)
    {
        //
    }

    //Select the target to suppress
    public void PaintTarget()
    {
        suppressionTarget = null;

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, 100f))
        {
            Unit_Master tempUnit;

            tempUnit = hit.collider.GetComponent<Unit_Master>();

            if (tempUnit != null)
            {
                suppressionTarget = tempUnit;
            }
        }
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
        TrackSuppressTarget();

        bool losCheck = false;

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, 100f))
        {
            if (suppressionTarget == hit.collider.GetComponent<Unit_Master>())
            {
                losCheck = true;
            }

            else
            {
                losCheck = false;
            }

        }

        if (shooting.isFiring == false && losCheck == true)
        {
            shooting.TestShooting(.9f);
        }
    }

    public void SpendMovement()
    {
        movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.x - movementPosition.x);
        //movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.y - movementPos.y);
        movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.z - movementPosition.z);

        movementPosition = this.transform.position;

        if (movementPointsRemaining <= 0)
        {
            movementPointsRemaining = 0;
            KD_CC.cantMove = true;
        }
    }

    public void ResetMovement()
    {
        movementPointsRemaining = startingMovementPoints;
        hasNoMovementRemaining = false;
        KD_CC.cantMove = false;
    }

    public virtual void Die()
    {

    }
    #endregion
} 