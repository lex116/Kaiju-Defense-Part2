using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{ 
    [HideInInspector]
    KD_CharacterController KD_CC;
    public Vehicle pilotedVehicle;
    public int InitiativeValue;
    public Shooting shooting;
    public bool IsBeingControlled;
    public Animator ShootingStateMachine;
    public Unit TargetUnit;

    public void Awake()
    {
        KD_CC = GetComponent<KD_CharacterController>();
        shooting = GetComponent<Shooting>();
        ShootingStateMachine = GetComponent<Animator>();
    }

    public void ToggleControl(bool toggle)
    {
        KD_CC.IsBeingControlled = toggle;
        KD_CC.playerCamera.SetActive(toggle);
    }

    public void Update()
    {
        PlayerInput();
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
            ShootingStateMachine.SetInteger("ShootingMode", 3);
        }
    }
}