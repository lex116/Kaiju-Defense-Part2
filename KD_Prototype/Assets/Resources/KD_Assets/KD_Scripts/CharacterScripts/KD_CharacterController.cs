using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KD_CharacterController : MonoBehaviour
{
    public bool cantMove;
    public bool cantLook;

    #region MouseFields
    public GameObject AimingNode;
    internal float mouseSensitivity = 1;
    internal float xAxisClamp = 0.0f;
    public float xRotMaxUp = -90;
    public float xRotMinDown = 90;
    #endregion 

    #region MovementFields
    internal CharacterController characterController;
    internal float walkSpeed = 6;
    internal Rigidbody rigidBody;
    float GroundCheckDistance = 0.75f;
    #endregion

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();
    }

    #region Methods

    // Use this for every frame jolly good tip tip
    public virtual void InputUpdate()
    {
        if (!cantMove)
        {
            MovePlayer();
        }

        if (!cantLook)
        {
            RotateCamera();
        }

        GroundCheck();
    }

    // Looking around via mouse
    public void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;

        Vector3 targetRotCam = AimingNode.transform.rotation.eulerAngles;
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

        AimingNode.transform.rotation = Quaternion.Euler(targetRotCam);
        transform.rotation = Quaternion.Euler(targetRotBody);
    }

    // Moves the character
    public void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

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
}