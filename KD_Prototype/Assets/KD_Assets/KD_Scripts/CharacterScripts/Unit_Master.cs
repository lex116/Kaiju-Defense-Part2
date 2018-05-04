using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Master : MonoBehaviour, IDamagable
{
    public enum FactionTag
    {
        Echo,
        Shades,
        Russia
    }

    [SerializeField]
    public FactionTag UnitStat_FactionTag;


    public enum DemoWeapon
    {
        Human_Pistol,
        Human_Shotgun,
        Human_MachineGun,
        Vehicle_MachineGun
    }

    [SerializeField]
    public DemoWeapon selectedWeapon;

    public enum Actions
    {
        QuickShot,
        AimedShot,
        SuppressShot
    }

    public Actions SelectedAction;

    #region Component Fields
    public KD_CharacterController KD_CC;
    [HideInInspector]
    public Shooting shooting;
    public bool IsBeingControlled;
    public Animator ShootingStateMachine;
    //This doesnt do anything but it could if we change the way targetting works
    public Unit_Master TargetUnit;
    public GameObject AimingNode;
    //public GameObject Camera;
    public Camera playerCamera;
    internal RoundManager roundManager;
    [SerializeField]
    public Weapon currentWeapon;
    public Unit_Master suppressionTarget;
    public string LookedAtObject;
    int QuickShotFOV = 60;
    int AimedShotFOV = 15;
    int SuppressShotFOV = 30;
    int TargetFOV;
    public bool isAbleToSuppress;

    #endregion

    #region Unit Stats
    public string UnitStat_Name;
    public int UnitStat_Initiative;
    //[HideInInspector]
    internal int UnitStat_StartingHitPoints;
    internal int UnitStat_HitPoints;
    public int UnitStat_StartingActionPoints;
    public int UnitStat_ActionPoints;
    public int UnitStat_Reaction;
    //Temp Value
    public int UnitStat_Accuracy;
    //
    public int UnitStat_Willpower;
    public int UnitStat_Fitness;
    public float UnitStat_Speed;
    //public int UnitStat_Aptitude; ???
    public int UnitStat_VisualRange;
    public int UnitStat_StartingNerve;
    int UnitStat_Nerve;
    bool isPanicked;
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
        //Camera.SetActive(toggle);
        playerCamera.gameObject.SetActive(toggle);
        IsBeingControlled = toggle;
        SetWeapon();
        SetAction_QuickShot();
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
        {
            SetAction_QuickShot();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetAction_AimedShot();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetAction_SuppressShot();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (SelectedAction == Actions.QuickShot)
            {
                ShootingStateMachine.SetInteger("ShootingMode", 1);
                roundManager.HUD_Player_ConfirmText.SetActive(true);
            }

            if (SelectedAction == Actions.AimedShot)
            {
                ShootingStateMachine.SetInteger("ShootingMode", 2);
                roundManager.HUD_Player_ConfirmText.SetActive(true);
            }

            if (SelectedAction == Actions.SuppressShot)
            {
                PaintTarget();

                if (suppressionTarget != null)
                {
                    ShootingStateMachine.SetInteger("ShootingMode", 3);
                    roundManager.HUD_Player_ConfirmText.SetActive(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && KD_CC.characterController.isGrounded)
        {
            roundManager.HUD_Player_ConfirmText.SetActive(true);
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

        if (isPanicked)
        {
            Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * 0.75f;
        }
    }

    public void SetAction_QuickShot()
    {
        SelectedAction = Actions.QuickShot;
        roundManager.HUD_Player_ActionText.text = "QuickShot";
        TargetFOV = QuickShotFOV;
    }

    public void SetAction_AimedShot()
    {
        SelectedAction = Actions.AimedShot;
        roundManager.HUD_Player_ActionText.text = "AimedShot";
        TargetFOV = AimedShotFOV;
    }

    public void SetAction_SuppressShot()
    {
        SelectedAction = Actions.SuppressShot;
        roundManager.HUD_Player_ActionText.text = "SuppressShot";
        TargetFOV = SuppressShotFOV;
    }

    public void ChangeCameraFOV()
    {
        if (playerCamera.fieldOfView < TargetFOV)
        {
            playerCamera.fieldOfView++;
        }

        if (playerCamera.fieldOfView > TargetFOV)
        {
            playerCamera.fieldOfView--;
        }
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

        RaycastHit hit;

        if (Physics.Raycast(AimingNode.transform.position, AimingNode.transform.forward, out hit, currentWeapon.Range))
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
        if (isAbleToSuppress == true)
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
            if (KD_CC.characterController.isGrounded)
            {
                KD_CC.cantMove = true;
            }
        }
    }

    public void ResetMovement()
    {
        movementPointsRemaining = startingMovementPoints;
        hasNoMovementRemaining = false;
        KD_CC.cantMove = false;
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