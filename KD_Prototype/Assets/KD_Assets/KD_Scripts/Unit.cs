using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamagable
{
    #region Component Fields
    [HideInInspector]
    KD_CharacterController KD_CC;
    public Vehicle pilotedVehicle;
    public Shooting shooting;
    public bool IsBeingControlled;
    public Animator ShootingStateMachine;
    //This doesnt do anything but it could if we change the way targetting works
    public Unit TargetUnit;
    public GameObject AimingNode;
    public GameObject Camera;
    RoundManager RM;
    [SerializeField]
    public Weapon currentWeapon;
    public Unit suppressTarget;
    #endregion

    #region Unit Stats
    public int InitiativeValue;
    int UnitStat_HitPoints = 5;
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
    public bool isDead;
    public Vector3 movementPos;
    public float movementPointsRemaining;
    public bool hasNoMovementRemaining;
    #endregion

    #region Calculated Weapon Stats
    //public int Calculated_WeaponDamage;
    public float Calculated_WeaponAccuracy;
    #endregion

    #region Utility + Setup Methods
    public void Awake()
    {
        KD_CC = GetComponent<KD_CharacterController>();
        shooting = GetComponent<Shooting>();
        shooting.unit = this;
        ShootingStateMachine = GetComponent<Animator>();
        RM = FindObjectOfType <RoundManager>();

        currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Pistol");
        //currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Shotgun");

        movementPos = this.transform.position;
    }

    public void ToggleControl(bool toggle)
    {
        //KD_CC.AimingNode.SetActive(toggle);
        Camera.SetActive(toggle);
        IsBeingControlled = toggle;
    }

    public void Update()
    {
        if (IsBeingControlled)
        {
            PlayerInput();
            SpendMovement();
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

            if (suppressTarget != null)
            {
                ShootingStateMachine.SetInteger("ShootingMode", 3);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RM.EndUnitTurn();
        }
    }
    #endregion

    #region Combat Methods
    public virtual void TakeDamage(int Damage)
    {
        if (isDead == false)
        {
            UnitStat_HitPoints = UnitStat_HitPoints - Damage;
            if (UnitStat_HitPoints <= 0)
            {
                DIE();
            }
        }
    }

    public void DIE()
    {
        Debug.Log(this.gameObject.name + "has died");

        isDead = true;

        if (RM.SelectedUnit == this)
        {
            RM.EndUnitTurn();
        }
    } 

    void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = UnitStat_Accuracy * currentWeapon.Accuracy;
    }

    //Select the target to suppress
    public void PaintTarget()
    {
        suppressTarget = null;

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, 100f))
        {
            Unit tempUnit;

            tempUnit = hit.collider.GetComponent<Unit>();

            if (tempUnit != null)
            {
                suppressTarget = tempUnit;
            }
        }
    }

    public void TrackSuppressTarget()
    {
        //Rotate the body to face the target
        transform.LookAt(suppressTarget.transform);

        Vector3 bodyEulerAngles = transform.rotation.eulerAngles;
        bodyEulerAngles.x = 0;
        bodyEulerAngles.z = 0;

        transform.rotation = Quaternion.Euler(bodyEulerAngles);

        //Rotate the camera to face the target
        AimingNode.transform.LookAt(suppressTarget.transform);

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
            if (suppressTarget == hit.collider.GetComponent<Unit>())
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
            shooting.TestShooting();
        }
    }

    public void SpendMovement()
    {
        movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.x - movementPos.x);
        movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.y - movementPos.y);
        movementPointsRemaining = movementPointsRemaining - Mathf.Abs(this.transform.position.z - movementPos.z);

        movementPos = this.transform.position;

        if (movementPointsRemaining <= 0)
        {
            KD_CC.cantMove = true;
        }
    }
    #endregion
} 