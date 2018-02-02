using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharController : MonoBehaviour
{
    #region Setup
    Health HealthScript;
    Player_Inventory Player_InventoryScript;
    public string PlayerNumber;

    [SerializeField]
    GameObject Arms;
    [SerializeField]
    GameObject Gyro;
    Rigidbody rbGyro;
    Rigidbody rb;
    CapsuleCollider capCollider;

    [SerializeField]
    GameObject Rightarm;
    [SerializeField]
    GameObject Leftarm;
    bool RightArmWaiting;
    bool LeftArmWaiting;
    #endregion
    #region Movement
    //10 for now
    float MovementSpeed = 10;
    float inputLimiter = 25;
    float FrictionPercentage = 75;
    float targetMoveSpeed;
    float ForwardLimit = 5.75f;
    float StrafeLimit = 5.75f;
    float BackLimit = 4.25f;

    float XinputMove;
    float YinputMove;

    float LimitX;
    float LimitY;
    float LimitZ;

    float MoveDeadZone = 0.25f;
    #endregion
    #region Look
    //2.5 for now
    float LookSensivity = 2.5f;
    float MaximumYRotation = 275;
    float MinimumYRotation = 45;
    float XinputLook;
    float YinputLook;
    #endregion
    #region Jumping
    float GroundCheckLength = 0.175f;
    float ShellOffSet = 0.1f;
    float JumpForce = 400;
    Vector3 GroundNormal;

    public bool inAir;
    public float JumpCoolDownTimer = 1;
    public bool JumpCooledDown;
    #endregion

    float GravForce = 2.5f;

    #region Setup Functions
    public void Setup()
    {
        HealthScript = GetComponent<Health>();
        Player_InventoryScript = GetComponent<Player_Inventory>();
    }

    // Use this for initialization
    void Awake()
    {
        Setup();
    }

    void Start()
    {
        capCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        rbGyro = Gyro.GetComponent<Rigidbody>();
        JumpCooledDown = true;
    }
    #endregion

    //void Update()
    void FixedUpdate()
    {
        if (Player_InventoryScript.healthScript.isDestroyed == false)
        {
            //GC
            GroundCheck();

            //Axis
            Look();
            Move();
            Fire();
            Throw();

            //Button
            Jump();
            PlaceArmor();
            Activate();
            SwitchWeapon();
            Reload();
            Ability();
            //Fun
            TurnArmsOn();
            CustomGravity();
        }
    }
    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, capCollider.radius * (1.0f - ShellOffSet), Vector3.down, out hit,
        ((capCollider.height / 2f) - capCollider.radius) + GroundCheckLength))
        {
            if (hit.collider.gameObject.layer == 15|| hit.collider.gameObject.layer == 12)
            {
                GroundNormal = hit.normal;
                inAir = false;
            }
        }

        else
        {
            inAir = true;
        }
    }
    void Look()         //Right stick
    {
        if (Input.GetAxis("Right Joystick X" + PlayerNumber) != 0
            || Input.GetAxis("Right Joystick Y" + PlayerNumber) != 0)
        {
            XinputLook = Input.GetAxis("Right Joystick X" + PlayerNumber);
            YinputLook = Input.GetAxis("Right Joystick Y" + PlayerNumber);


            Arms.transform.Rotate
                (0f, XinputLook * LookSensivity, 0f, Space.World);

            Gyro.transform.Rotate
                (0f, XinputLook * LookSensivity, 0f, Space.World);

            Arms.transform.Rotate
                (YinputLook * LookSensivity, 0f, 0f, Space.Self);

            if (Arms.transform.eulerAngles.x >= MinimumYRotation 
                && Arms.transform.eulerAngles.x < MaximumYRotation 
                && YinputLook > 0) 
            {
                Arms.transform.eulerAngles = new Vector3(
                MinimumYRotation,
                Arms.transform.eulerAngles.y,
                0);
            }

            if (Arms.transform.eulerAngles.x <= MaximumYRotation 
                && Arms.transform.eulerAngles.x > MinimumYRotation 
                && YinputLook < 0) 
            {
                Arms.transform.eulerAngles = new Vector3(
                MaximumYRotation,
                Arms.transform.eulerAngles.y,
                0);
            }
        }
    }
    void Move()         //Left Stick
    {
        XinputMove = Input.GetAxis("Left Joystick X" + PlayerNumber);
        YinputMove = Input.GetAxis("Left Joystick Y" + PlayerNumber) * -1;
        XinputMove = XinputMove * (inputLimiter / 100);
        YinputMove = YinputMove * (inputLimiter / 100);
        

        if ((XinputMove != 0 ||
             YinputMove != 0) && (inAir == false))
        { 
            #region Moving
            if (XinputMove > 0 || XinputMove < 0)
            {
                targetMoveSpeed = StrafeLimit;
            }
            if (YinputMove < 0)
            {
                targetMoveSpeed = BackLimit;
            }
            if (YinputMove > 0)
            {
                targetMoveSpeed = ForwardLimit;
            }

            Vector3 targetMove = Gyro.transform.forward * YinputMove + Gyro.transform.right * XinputMove;
            targetMove = Vector3.ProjectOnPlane(targetMove, GroundNormal).normalized;

            targetMove.x = targetMove.x * targetMoveSpeed;
            targetMove.z = targetMove.z * targetMoveSpeed;
            targetMove.y = targetMove.y * targetMoveSpeed;

            //if (rb.velocity.sqrMagnitude < (targetMoveSpeed * targetMoveSpeed))
            {
                rb.AddForce(targetMove * MovementSpeed);
            }
            #endregion
        }

        if ((Math.Abs(XinputMove) <= MoveDeadZone &&
             Math.Abs(YinputMove) <= MoveDeadZone) && (inAir == false))
        {
            rb.AddForce(-rb.velocity * (MovementSpeed * (FrictionPercentage/100)));
        }
    }
    void Fire()         //Right Trigger
    {
        if (Input.GetAxis("Right Trigger" + PlayerNumber) > 0 && Player_InventoryScript.EquippedRangedWeaponScipt != null)
        {
            Player_InventoryScript.EquippedRangedWeaponScipt.Fire();
        }
    }
    void Throw()        //Left Trigger
    {
        if (Input.GetAxis("Left Trigger" + PlayerNumber) > 0 && (Player_InventoryScript.EquippedThrowableScript != null))
        {
            Player_InventoryScript.ThrowThrowable();
        }
    }
    void Jump()         //A Button
    {
        if (Input.GetButtonDown("A Button" + PlayerNumber) && inAir == false && JumpCooledDown == true)
        {
            rb.AddForce(transform.up * JumpForce);
            JumpCooledDown = false;
            StartCoroutine(JumpCoolDownRoutine());
        }
    }
    IEnumerator JumpCoolDownRoutine()
    {
        yield return new WaitForSeconds(JumpCoolDownTimer);
        JumpCooledDown = true;
    }
    void PlaceArmor()   //B Button
    {
        if (Input.GetButtonDown("B Button" + PlayerNumber))
        {
            Player_InventoryScript.PlaceArmor();
        }
    }
    void Activate()     //X Button
    {
        if (Input.GetButtonDown("X Button" + PlayerNumber))
        {
            if (Player_InventoryScript.InteractableToActivate != null)
            {
                Player_InventoryScript.InteractableToActivate.Activate(Player_InventoryScript);
            }
        }
    }
    void SwitchWeapon() //Y Button
    {
        if (Input.GetButtonDown("Y Button" + PlayerNumber))
        {
            Player_InventoryScript.ChangeWeapon();
        }
    }
    void Reload()       //Right Bumper
    {
        if (Input.GetButtonDown("Right Bumper" + PlayerNumber) && Player_InventoryScript.EquippedRangedWeaponScipt != null)
        {
            Player_InventoryScript.EquippedRangedWeaponScipt.Reload();
        }
    }
    void Ability()      //Left Bumper
    {
        if (Input.GetButtonDown("Left Bumper" + PlayerNumber))
        {
            //We don't have an ability
        }
    }
    void TurnArmsOn()   //Code Block that makes not want to die
    {
        if (Player_InventoryScript.EquippedRangedWeaponScipt != null && RightArmWaiting == true)
        {
            Rightarm.SetActive(true);
            RightArmWaiting = false;
        }
        if (Player_InventoryScript.EquippedRangedWeaponScipt == null && RightArmWaiting == false)
        {
            Rightarm.SetActive(false);
            RightArmWaiting = true;
        }

        if (Player_InventoryScript.EquippedThrowableScript != null && LeftArmWaiting == true)
        {
            Leftarm.SetActive(true);
            LeftArmWaiting = false;
        }
        if (Player_InventoryScript.EquippedThrowableScript == null && LeftArmWaiting == false)
        {
            Leftarm.SetActive(false);
            LeftArmWaiting = true;
        }
    }
    void CustomGravity()
    {
        if (inAir)
        {
            rb.AddForce(transform.up * -1 * GravForce);
        }
    }
}

