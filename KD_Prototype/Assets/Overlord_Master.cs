using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlord_Master : MonoBehaviour
{
    public Manager_GameState manager_GameState;
    public Manager_HUD manager_HUD;

    public KD_Global.FactionTag overlordFactionTag;
    //currentControlledUnit = ccu
    public Unit_Master ccu;
    CharacterController ccu_CharacterController;
    Camera ccu_camera;
     
    public bool isControllingUnit;

    internal Unit_Master LookedAtUnit_Master;
    internal Unit_VehicleHardPoint LookedAtUnit_VehicleHardPoint;

    [SerializeField]
    List<Unit_Master> horde_AllUnits = new List<Unit_Master>();

    public void SetManagers(Manager_GameState gameState, Manager_HUD hud)
    {
        manager_GameState = gameState;
        manager_HUD = hud;
    }

    public int InteractionRange = 2;

    LayerMask rayMask = ~(1 << 14);

    #region MouseFields
    public GameObject aimingNode;
    internal float mouseSensitivity = 1;
    internal float xAxisClamp = 0.0f;
    internal float xRotMaxUp = -90;
    internal float xRotMinDown = 90;
    #endregion 

    #region MovementFields
    internal CharacterController characterController;
    internal float walkSpeed = 6;
    internal Rigidbody rigidBody;
    float GroundCheckDistance = 0.75f;

    public float tempOverwrite = 0;
    #endregion

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();
    }

    public void AddUnitToHorde(Unit_Master unitToAdd)
    {
        horde_AllUnits.Add(unitToAdd);
    }


    #region Methods

    public void GroundCheckUpdate()
    {
        GroundCheck();
    }

    public virtual void MovementUpdate()
    {
        MovePlayer();
    }

    public virtual void LookUpdate()
    {
        RotateCamera();
    }

    // Looking around via mouse
    public void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

        Vector3 targetRotCam = aimingNode.transform.rotation.eulerAngles;
        Vector3 targetRotBody = transform.rotation.eulerAngles;

        targetRotCam.x -= rotAmountY;
        targetRotCam.z = 0;

        targetRotBody.y += rotAmountX;

        if (xAxisClamp > xRotMinDown)
        {
            xAxisClamp = xRotMinDown;
            targetRotCam.x = xRotMinDown;
        }

        else if (xAxisClamp < xRotMaxUp)
        {
            xAxisClamp = xRotMaxUp;
            targetRotCam.x = -3 * xRotMaxUp;
        }

        aimingNode.transform.rotation = Quaternion.Euler(targetRotCam);
        transform.rotation = Quaternion.Euler(targetRotBody);
    }

    // Moves the character
    public void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (tempOverwrite != 0)
            vertical = tempOverwrite;

        Vector3 moveDirSide = transform.right * horizontal * walkSpeed;
        Vector3 moveDirForward = transform.forward * vertical * walkSpeed;

        characterController.SimpleMove(moveDirSide);
        characterController.SimpleMove(moveDirForward);
    }

    // Sticks the character to the ground to prevent the skipping bug
    public bool GroundCheck()
    {
        if (characterController.isGrounded)
        {
            return true;
        }

        Vector3 bottom = characterController.transform.position
            - new Vector3(0, characterController.height / 2, 0);

        RaycastHit hit;

        if (Physics.Raycast(bottom, new Vector3(0, -1, 0), out hit, GroundCheckDistance))
        {
            characterController.Move(new Vector3(0, -hit.distance, 0));
            return true;
        }

        return false;
    }

    #endregion

    public void ToggleControlOfUnit(Unit_Master unitToControl, bool toggle)
    {
        ccu = unitToControl;
        ccu_CharacterController = unitToControl.gameObject.GetComponent<CharacterController>();

        ccu.playerCamera.gameObject.SetActive(toggle);
        //temp
        ccu.HeldWeapon_GunPrefab.gameObject.SetActive(toggle);
        ccu.CameraAudioListener.enabled = toggle;

        ccu.IsBeingControlled = toggle;

        if (toggle == true)
        {
            ccu.Current_Unit_State = Unit_Master.Unit_States.State_Moving;
            ccu.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_Waiting;
            ccu.Selected_Unit_Action = ccu.Unit_Actions[1];
            ccu.ResetAP();
            ccu.CalculateWeaponStats();
            ccu.CalculateCarryWeight();
            ccu.SuppressionCharges = ccu.StartingSuppressionCharges;

            manager_HUD.Player_HUD_Basic.SetActive(true);
            manager_HUD.Player_HUD_Moving.SetActive(true);
            manager_HUD.Player_HUD_Action.SetActive(false);
            manager_HUD.Player_HUD_Shooting.SetActive(false);
            manager_HUD.Player_HUD_Equipment.SetActive(false);
        }

        if (toggle == false)
        {
            ccu.Current_Unit_State = Unit_Master.Unit_States.State_Waiting;
        }
    }

    //public void FixedUpdate()
    //{
                //Input here
    //}

    public virtual void OverlordInput()
    {

    }

    
    //public virtual void Interaction()
    //{
    //    //
    //}

    public virtual void ToggleMovingState()
    {
        if (ccu.Current_Unit_State == Unit_Master.Unit_States.State_Moving)
        {
            manager_HUD.Player_HUD_Moving.SetActive(false);
            manager_HUD.Player_HUD_Action.SetActive(true);
            ccu.Current_Unit_State = Unit_Master.Unit_States.State_PreparingToAct;
            ChangeAction(1);
        }

        else if (ccu.Current_Unit_State == Unit_Master.Unit_States.State_PreparingToAct)
        {
            manager_HUD.Player_HUD_Moving.SetActive(true);
            manager_HUD.Player_HUD_Action.SetActive(false);
            ccu.Current_Unit_State = Unit_Master.Unit_States.State_Moving;
            ChangeAction(1);
        }
    }

    public void UseSelectedAction()
    {
        if (ccu.AP >= ccu.Selected_Unit_Action.Action_AP_Cost 
            && ccu.Selected_Unit_Action.CheckRequirements(ccu) == true)
        {
            ccu.AP = ccu.AP - ccu.Selected_Unit_Action.Action_AP_Cost;
            ccu.Selected_Unit_Action.Action_Effect(ccu);
            manager_GameState.RechargeAllSuppressors(ccu);
        }
        else
            manager_HUD.AddNotificationToFeed("Can't use that action!");
    }
    public void ChangeAction(int Selection)
    {
        if (ccu.Selected_Unit_Action != null)
            ccu.Selected_Unit_Action.Deselection_Effect(ccu);

        ccu.Selected_Unit_Action = ccu.Unit_Actions[Selection];
        ccu.Selected_Unit_Action.Selection_Effect(ccu);
    }

    public void FixedUpdate()
    {
        GroundCheckUpdate();

        if (ccu.Current_Unit_State != Unit_Master.Unit_States.State_Dying)
           SpendMovement();

        if (ccu.Current_Unit_State == Unit_Master.Unit_States.State_Moving &&
            ccu.movementPointsRemaining > 0)
            MovementUpdate();

        if (ccu.Current_Unit_State == Unit_Master.Unit_States.State_Moving ||
            ccu.Current_Unit_State == Unit_Master.Unit_States.State_PreparingToAct)
            LookUpdate();

        foreach (Unit_Master x in horde_AllUnits)
        {
            if (x.Current_Unit_Suppression_State == Unit_Master.Unit_Suppression_States.State_Suppressing &&
                x.isDead == false)
            {
                x.SuppressionUpdate();
            }    
        }
    }

    public void Update()
    {
        if (ccu.IsBeingControlled && ccu.isDead == false)
        {
            //come back to this when input is getting ready to be set up
            //PlayerInput();
            LookAtTarget();
        }

        //refer to unit_master
        //OrientUnitIcon();
    }

    public virtual void LookAtTarget()
    {
        LookedAtUnit_Master = null;
        LookedAtUnit_VehicleHardPoint = null;

        RaycastHit hit;

        if (Physics.Raycast(aimingNode.transform.position, aimingNode.transform.forward, out hit, ccu.equippedWeapon.Range))
        {
            if (hit.collider.gameObject.name != null)
            {
                LookedAtUnit_Master = hit.collider.gameObject.GetComponent<Unit_Master>();
                LookedAtUnit_VehicleHardPoint = hit.collider.gameObject.GetComponent<Unit_VehicleHardPoint>();
            }
        }
    }

    public void SpendMovement()
    {
        if (characterController.isGrounded)
        {
            ccu.movementPointsRemaining = ccu.movementPointsRemaining - Mathf.Abs(this.transform.position.x - ccu.movementPosition.x);
            ccu.movementPointsRemaining = ccu.movementPointsRemaining - Mathf.Abs(this.transform.position.z - ccu.movementPosition.z);
        }

        ccu.movementPosition = this.transform.position;

        if (ccu.movementPointsRemaining <= 0)
        {
            ccu.movementPointsRemaining = 0;
        }
    }

    //Select the target to suppress
    public void PaintTarget()
    {
        ccu.suppressionTarget = null;

        Unit_Master possibleSuppressionTarget = null;

        if (LookedAtUnit_Master != null)
            possibleSuppressionTarget = LookedAtUnit_Master;

        if (LookedAtUnit_VehicleHardPoint != null)
            possibleSuppressionTarget = LookedAtUnit_VehicleHardPoint.GetComponentInParent<Unit_Master>();

        if (possibleSuppressionTarget != null)
        {
            if (possibleSuppressionTarget.characterSheet.UnitStat_Initiative < ccu.characterSheet.UnitStat_Initiative)
                ccu.suppressionTarget = possibleSuppressionTarget;

            else
                manager_HUD.AddNotificationToFeed("Can't suppress " + possibleSuppressionTarget.characterSheet.UnitStat_Name);
        }
    }

    //this is totally fucked so fix it at some point in the future but it's basically completely fucking useless anyway
    public virtual void Interaction()
    {
        IInteractable objectToActivate = null;

        RaycastHit hit;

        if (Physics.Raycast(aimingNode.transform.position, aimingNode.transform.forward, out hit, InteractionRange, rayMask))
        {
            objectToActivate = hit.collider.GetComponent<IInteractable>();

            if (objectToActivate == null)
            {
                objectToActivate = hit.collider.GetComponentInParent<IInteractable>();
            }

            if (objectToActivate != null)
            {
                // this needs to get fixed whenever the fuck interaction gets fixed
                //objectToActivate.Activate(ccu);
            }
        }
    }
}
