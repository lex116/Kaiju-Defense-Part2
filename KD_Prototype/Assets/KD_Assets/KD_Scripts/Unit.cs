using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IDamagable
{ 
    [HideInInspector]
    KD_CharacterController KD_CC;
    public Vehicle pilotedVehicle;
    public int InitiativeValue;
    public Shooting shooting;
    public bool IsBeingControlled;
    public Animator ShootingStateMachine; 
    public Unit TargetUnit;
    public GameObject Camera;

    RoundManager RM;

    #region Unit Stats
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
    #endregion

    public bool isDead;

    [SerializeField]
    public Weapon currentWeapon;

    #region Calculated Weapon Stats
    //public int Calculated_WeaponDamage;
    public float Calculated_WeaponAccuracy;
    #endregion



    //Suppress fire fields
    public Unit suppressTarget;
    public bool isSuppressing;



    public void Awake()
    {
        KD_CC = GetComponent<KD_CharacterController>();
        shooting = GetComponent<Shooting>();
        shooting.unit = this;
        ShootingStateMachine = GetComponent<Animator>();
        RM = FindObjectOfType <RoundManager>();

        currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Pistol");
        //currentWeapon = (Weapon)ScriptableObject.CreateInstance("Human_Shotgun");
    }

    public void ToggleControl(bool toggle)
    {
        KD_CC.IsBeingControlled = toggle;
        KD_CC.playerCamera.SetActive(toggle);
        IsBeingControlled = toggle;
    }

    public void Update()
    {
        if (IsBeingControlled)
        {
            PlayerInput();
        }

        if (isSuppressing)
        {
            SuppressFire();
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
            RM.FindNextActionsToActivate();
            RM.ActivateActions();
        }
    }

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

        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, 100f))
        {
            Unit tempUnit;

            tempUnit = hit.collider.GetComponent<Unit>();

            if (tempUnit != null)
            {
                suppressTarget = tempUnit;
            }
        }
    }


    public void ToggleSuppressFire(bool toggle)
    {
        isSuppressing = toggle;
    }


    public void SuppressFire()
    {
        //TEMP
        // change to only rotate on the y for the body and the on the x for the camera
        transform.LookAt(suppressTarget.transform);
    }
} 