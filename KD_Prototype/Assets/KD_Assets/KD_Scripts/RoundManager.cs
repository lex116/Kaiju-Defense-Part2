using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour 
{
    #region InitiativeFields
    public Unit SelectedUnit;
    public Unit[] AllUnits;
    public int SelectedUnitIndex;
    [SerializeField]
    List<Unit> initiativeOrder = new List<Unit>();
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
        if (Input.GetMouseButton(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
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
    }
    #region Start Round Methods
    //2a. Find all units in the scene, order them by initiative ignoring the dead units
    void OrderUnitsByInitiative()
    {
        AllUnits = null;

        Unit[] tempUnits;

        tempUnits = null;

        tempUnits = FindObjectsOfType<Unit>();

        AllUnits = tempUnits.OrderByDescending(x => x.InitiativeValue).ToArray();

        initiativeOrder.Clear();

        foreach (Unit x in AllUnits)
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
        foreach (Unit x in AllUnits)
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

        foreach (Unit x in initiativeOrder)
        {
            if (x != SelectedUnit)
            {
                x.ToggleControl(false);
            }
        }

        SelectedUnit.ToggleControl(true);
    }

    //2d. Restore all movement points to all units and reset can move bool
    void ResetUnitMovement()
    {
        foreach (Unit x in initiativeOrder)
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

                ActivateNewUnit();
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
        SelectedUnit.ToggleControl(true);
        ActivateSuppressors();
    }
    //Activates the units who are suppress firing the controlled unit
    public void ActivateSuppressors()
    {
        foreach (Unit x in initiativeOrder)
        {
            if (x.suppressTarget == SelectedUnit)
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
        foreach (Unit x in initiativeOrder)
        {
            x.ShootingStateMachine.SetInteger("ShootingMode", 0);
            x.ShootingStateMachine.SetBool("isSPR", false);
            x.ShootingStateMachine.SetBool("Reset", true);
        }
    }
    #endregion
}