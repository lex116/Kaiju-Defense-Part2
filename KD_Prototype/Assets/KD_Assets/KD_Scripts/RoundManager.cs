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
    //Unit[] AllUnits;
    public int SelectedUnitIndex;
    //public Unit[] initiativeOrder;
    [SerializeField]
    List<Unit> initiativeOrder = new List<Unit>();
    //public bool isCompletingActions;
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

    #region RoundFields
    int RoundCount = 0;
    #endregion

    [SerializeField]
    List<Unit> SuppressingUnits = new List<Unit>();

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    #region //Round Flow Methods
    // Use this for initialization
    void Start ()
    {
        StartBattle();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButton(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    //Begin the inital round and set up, then call startRound
    void StartBattle()
    {
        StartRound();
    }

    //Initial round set up
    void StartRound()
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

        foreach (Unit x in AllUnits)
        {
            if (x.isDead == true)
            {
                Destroy(x.gameObject);
            }
        }
    }

    //Activate the unit to be controlled
    void ActivateNewUnit()
    {
        SelectedUnit.ToggleControl(true);
    }

    //Selects next unit
    void SelectNextUnit()
    {
        SelectedUnitIndex++;

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

    //Any clean up, then call RoundProcess
    void EndRound()
    {
        ClearOldActions();
        Debug.Log("round end");

        StartRound();
    }

    //get next unit, move time scale forward
    public void EndUnitTurn()
    {
        FindNextActionsToActivate();
    }

    //activate all remaining actions
    void ActivateRemainingActions()
    {
        Debug.Log("Round Finished");
    }

    #endregion

    #region //TimeScale Functions

    // Adds an action to the list to be called later, sets it priority
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

    //Removes used up actions from the timescale
    public void ClearOldActions()
    {
        TimeScaleOrder.RemoveAll(TimeScaleAction => TimeScaleAction.timeScalePosition <= CurrentTime);
    }

    // Steps the timescale forward
    void IncrememntTime()
    {
        CurrentTime++;
    }
    #endregion

    // Activates the units that need supression fire
    public void ActivateSuppressionCheck()
    {

    }
}