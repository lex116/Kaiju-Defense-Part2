using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour 
{
    public bool isInMapMode;

    #region Map Hud Fields
    [Header("Map Hud Fields")]
    public Camera MapCamera;
    public GameObject MapHUD;

    public Text HUD_Map_nameText;
    public Text HUD_Map_hpText;
    public Text HUD_Map_heldWeaponText;
    public Text HUD_Map_accText;

    #endregion

    #region Hud Fields
    [Header("Hud Fields")]
    public GameObject PlayerHUD;

    public Image HUD_Player_staminaBar;
    public Text HUD_Player_heldWeaponText;
    public Text HUD_Player_hpText;
    public Text HUD_Player_nameText;
    public Text HUD_Player_accText;
    public Text HUD_Player_lookAtText;
    #endregion

    #region InitiativeFields
    public Unit_Master SelectedUnit;
    //public Unit_Human SelectedUnit;
    public Unit_Master[] AllUnits;
    public int SelectedUnitIndex;
    [SerializeField]
    List<Unit_Master> initiativeOrder = new List<Unit_Master>();
    #endregion

    #region TimeScaleFields
    [SerializeField]
    public List<TimeScaleAction> TimeScaleOrder = new List<TimeScaleAction>();
    public int CurrentTime;
    [SerializeField]
    List<TimeScaleAction> QueuedActions = new List<TimeScaleAction>();
    [SerializeField]
    public TimeScaleAction[] ActionsToActivate;
    #endregion

    // Mouse Locking
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Input.GetMouseButton(0) && Cursor.lockState != CursorLockMode.Locked && isInMapMode == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (SelectedUnit != null && isInMapMode == false)
        {
            PlayerHudUpdate();
        }
    }
    void PlayerHudUpdate()
    {
        HUD_Player_staminaBar.fillAmount = SelectedUnit.movementPointsRemaining / SelectedUnit.startingMovementPoints;

        HUD_Player_heldWeaponText.text = SelectedUnit.currentWeapon.Weapon_Name;

        HUD_Player_hpText.text = "Hp : " + SelectedUnit.UnitStat_HitPoints;

        HUD_Player_nameText.text = SelectedUnit.gameObject.name;

        HUD_Player_accText.text = "Acc: " + SelectedUnit.Calculated_WeaponAccuracy.ToString("0%");

        HUD_Player_lookAtText.text = SelectedUnit.LookedAtObject;
    }

    //0. Calls StartBattle()
    void Start ()
    {
        StartBattle();
	}


    //1. Starts the battle scenario, playing a cinematic or text etc, then calls StartRound()
    void StartBattle()
    {
        StartRound();
    }


    //2. Order all units in the scene by initiative, remove dead units, select the first unit
    void StartRound()
    {
        //OrderUnitsByInitiative();
        //DestroyDeadUnits();
        //SelectTheFirstUnit();

        //Gives enough time for the input to go through and prevents the next character from being skipped
        StartCoroutine(SRTestRoutine());
    }
    IEnumerator SRTestRoutine()
    {
        yield return new WaitForSeconds(1f);
        OrderUnitsByInitiative();
        DestroyDeadUnits();
        SelectTheFirstUnit();
        ResetUnitMovement();

        ActivateMapCam();
    }
    #region Start Round Methods
    //2a. Find all units in the scene, order them by initiative ignoring the dead units
    void OrderUnitsByInitiative()
    {
        AllUnits = null;

        Unit_Master[] tempUnits;

        tempUnits = null;

        tempUnits = FindObjectsOfType<Unit_Master>();

        AllUnits = tempUnits.OrderByDescending(x => x.UnitStat_Initiative).ToArray();

        initiativeOrder.Clear();

        foreach (Unit_Master x in AllUnits)
        {
            if (x.isDead != true)
            {
                initiativeOrder.Add(x);
            }
        }
    }

    //2b. Remove all dead units from the scene
    void DestroyDeadUnits()
    {
        foreach (Unit_Master x in AllUnits)
        {
            if (x.isDead == true)
            {
                Destroy(x.gameObject);
            }
        }
    }

    //2c. Select the first unit to take control of
    void SelectTheFirstUnit()
    {
        SelectedUnit = null;

        SelectedUnitIndex = 0;

        SelectedUnit = initiativeOrder[SelectedUnitIndex];

        foreach (Unit_Master x in initiativeOrder)
        {
            if (x != SelectedUnit)
            {
                x.ToggleControl(false);
            }
        }

        //SelectedUnit.ToggleControl(true);
    }

    //2d. Restore all movement points to all units and reset can move bool
    void ResetUnitMovement()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.ResetMovement();
        }
    }
    #endregion


    //3. Unit adds an action to the timescale moving the round forward
    #region Units Adding Actions/ Unit Turns Methods
    // Adds an action to the list to be called later, sets it's priority
    public void AddAction(TimeScaleAction TSA_ToBeAdded)
    {
        SelectedUnit.IsBeingControlled = false;

        int priorityOffset = 0;

        TSA_ToBeAdded.timeScalePosition = TSA_ToBeAdded.timeScaleOffSet + CurrentTime;

        foreach (TimeScaleAction x in TimeScaleOrder)
        {
            if (x.timeScalePosition == TSA_ToBeAdded.timeScalePosition)
            {
                priorityOffset++;
            }
        }

        TSA_ToBeAdded.timeScalePriority = priorityOffset;

        TimeScaleOrder.Add(TSA_ToBeAdded);

        EndUnitTurn();
    }

    //Ends the unit's turn and calls FindNextActionsToActivate as a result
    public void EndUnitTurn()
    {
        FindNextActionsToActivate();
    }

    // Finds all the actions that should be called this time unit
    public void FindNextActionsToActivate()
    {
        QueuedActions.Clear();

        foreach (TimeScaleAction x in TimeScaleOrder)
        {
            if (x.timeScalePosition == CurrentTime)
            {
                QueuedActions.Add(x);
            }
        }

        ActionsToActivate = QueuedActions.OrderBy(x => x.timeScalePriority).ToArray();

        ActivateActions();
    }

    // Activates the actions that should be activated this time unit
    public void ActivateActions()
    {
        StartCoroutine(ActivateActionsRoutine());
    }
    IEnumerator ActivateActionsRoutine()
    {
        foreach (TimeScaleAction x in ActionsToActivate)
        {
            x.ActionEffect();

            while(x.ActingUnit.shooting.isFiring)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(1f);
        }

        ActionsToActivate = null;


        //Temp
        SelectNextUnit();
        IncrememntTime();
    }

    //Selects next unit in the initiative order
    void SelectNextUnit()
    {
        StartCoroutine(SelectNextUnitRoutine());
    }
    IEnumerator SelectNextUnitRoutine()
    {
        yield return new WaitForSeconds(1f);

        SelectedUnitIndex++;

        Debug.Log("unit index: " + SelectedUnitIndex);

        if (SelectedUnitIndex >= initiativeOrder.Count)
        {
            EndRound();
        }

        else
        {
            if (initiativeOrder[SelectedUnitIndex].isDead == false)
            {
                SelectedUnit = initiativeOrder[SelectedUnitIndex];

                for (int i = 0; i < initiativeOrder.Count; i++)
                {
                    if (initiativeOrder[i] != SelectedUnit)
                    {
                        initiativeOrder[i].ToggleControl(false);
                    }
                }

                //ActivateNewUnit();
                ActivateMapCam();
            }

            else
            {
                SelectNextUnit();
            }
        }
    }

    //Activate the unit to be controlled
    void ActivateNewUnit()
    {
        //SelectedUnit.ShootingStateMachine.SetInteger("ShootingMode", 0);
        //SelectedUnit.ShootingStateMachine.SetBool("isSPR", false);
        //SelectedUnit.ShootingStateMachine.SetBool("Reset", true);
        SelectedUnit.ToggleControl(true);
        ActivateSuppressors();
    }

    //Activates the units who are suppress firing the controlled unit
    public void ActivateSuppressors()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            if (x.suppressionTarget == SelectedUnit)
            {
                x.ShootingStateMachine.SetBool("isSPR", true);
            }
            else
            {
                x.ShootingStateMachine.SetBool("isSPR", false);
            }
        }
    }

    // Steps the timescale forward
    void IncrememntTime()
    {
        CurrentTime++;
    }
    #endregion


    //4. Once all units have had turn the RM cleans up and restarts at step 2.
    #region End Turn Methods
    //Any clean up, then call RoundProcess
    void EndRound()
    {
        //ActivateRemainingActions();
        ClearOldActions();
        ResetAllUnitStateMachines();
        Debug.Log("round end");

        StartRound();
    }
    //activate all remaining actions
    void ActivateRemainingActions()
    {
        Debug.Log("Round Finished");
    }
    //Removes used up actions from the timescale
    public void ClearOldActions()
    {
        TimeScaleOrder.RemoveAll(TimeScaleAction => TimeScaleAction.timeScalePosition <= CurrentTime);
    }
    //Sets all units back to waiting before the next round
    public void ResetAllUnitStateMachines()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.ShootingStateMachine.SetInteger("ShootingMode", 0);
            x.ShootingStateMachine.SetBool("isSPR", false);
            x.ShootingStateMachine.SetBool("Reset", true);
        }
    }
    #endregion

    public void ActivateMapCam()
    {
        ResetAllUnitStateMachines();

        isInMapMode = true;
        Cursor.lockState = CursorLockMode.None;

        PlayerHUD.gameObject.SetActive(false);

        MapCamera.gameObject.SetActive(true);
        MapHUD.SetActive(true);

        HUD_Map_nameText.text = SelectedUnit.gameObject.name;
        HUD_Map_hpText.text = "Hp : " + SelectedUnit.UnitStat_HitPoints;
        HUD_Map_heldWeaponText.text = SelectedUnit.currentWeapon.Weapon_Name;
        HUD_Map_accText.text = "Acc: " + SelectedUnit.Calculated_WeaponAccuracy.ToString("0%");

    }

    public void DeactivateMapCam()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInMapMode = false;


        PlayerHUD.gameObject.SetActive(true);

        MapCamera.gameObject.SetActive(false);
        MapHUD.SetActive(false);

        ActivateNewUnit();


    }
}