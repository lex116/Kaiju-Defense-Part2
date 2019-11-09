using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager_GameState : MonoBehaviour
{
    //nonfucntionl
    internal SpawnPoint[] spawnPointsInScenario;
    //

    bool isInStrategicMode;
    //bool isInMapMode;
    public Camera MapCamera;

    internal List<Unit_Master> allUnitsInScene = new List<Unit_Master>();
    internal List<Overlord_Master> allOverlordsInScene = new List<Overlord_Master>();

    //scenario class
    public Blueprint_Scenario_Master selectedScenario;

    Manager_HUD manager_HUD;

    #region InitiativeFields
    public Unit_Master selected_Unit;
    public Overlord_Master selected_Overlord;
    internal Unit_Master[] AllUnits;
    public int SelectedUnitIndex;
    [SerializeField]
    List<Unit_Master> initiativeOrder = new List<Unit_Master>();
    public bool TurnIsEnding;
    #endregion

    
    // set the selected unit and overlord by finding the selected unit's overlord
    // for the gamestate and hud manager
    public void SetSelectedOverlordAndUnit(int index)
    {
        selected_Unit = initiativeOrder[index];

        foreach (Overlord_Master x in allOverlordsInScene)
        {
            if (x.overlordFactionTag == selected_Unit.characterSheet.UnitStat_FactionTag)
            {
                selected_Overlord = x;
            }
        }

        manager_HUD.hud_SelectedOverlord = selected_Overlord;
    }

    //1. Update the hud as it changes
    //2. Check if the playe is dead
    //3. update the minimap size
    //4. check for the win condition of the scenario
    void Update()
    {
        if (selected_Overlord.ccu != null && isInStrategicMode == false)
        {
            manager_HUD.PlayerHudUpdate();
        }

        if (selected_Overlord.ccu != null && selected_Overlord.ccu.isDead == true && TurnIsEnding == false)
        {
            EndUnitTurn();
        }

        manager_HUD.ChangeMiniMapSize();

        CheckWinCondition();
    }

    //0 on awake lock frame rate and cursor
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //1 set scenario setup
    void SetBattleScenario(Blueprint_Scenario_Master scenario)
    {
        Overlord_Master tempOverlordReference;

        //read scenario blueprint from selected scenario class
        selectedScenario = scenario;

        //instantiate map
        Instantiate(selectedScenario.scenarioMap);

        //get all spawn locations in our scenario's map
        SpawnPoint[] tempAllspawns = FindObjectsOfType<SpawnPoint>();

        //generate a number of overlords for each faction in the scenario
        tempOverlordReference = Instantiate(selectedScenario.playerOverlord);
        tempOverlordReference.overlordFactionTag = selectedScenario.playerOverlordFaction;
        tempOverlordReference.SetManagers(this, manager_HUD);
        allOverlordsInScene.Add(tempOverlordReference);

        tempOverlordReference = Instantiate(selectedScenario.enemyOverlord);
        tempOverlordReference.overlordFactionTag = selectedScenario.enemyOverlordFaction;
        tempOverlordReference.SetManagers(this, manager_HUD);
        allOverlordsInScene.Add(tempOverlordReference);

        //generate units from unit blueprints from the scenario class
        foreach (Blueprint_Unit_Master y in selectedScenario.scenarioUnits)
        {
            //generate them at that the selected spawn poins listed in the scenario class
            KD_Global.SpawnUnit(y, tempAllspawns[selectedScenario.scenarioUnits.IndexOf(y)]);
        }

        // Determine initiative order, reset suppression, etc
        SetRound();
    }

    //tell the hud manager to reset somethings then start the round
    void SetRound()
    {
        manager_HUD.HUD_SetRound();
        StartRound();
    }








    






















    //2 start a round
    #region
    //Order all units in the scene by initiative, remove dead units, select the first unit
    void StartRound()
    {
        OrderUnitsByInitiative();
        DestroyDeadUnits();
        SelectTheFirstUnit();
        ResetUnitMovement();
        GiveAllUnitsNerve();
        //ActivateMapCam();
        StrategicModeToggle(true);
    }



    //Find all units in the scene, order them by initiative ignoring the dead units
    //also give this to the hud manager so we don't have to reference it a thousand times
    void OrderUnitsByInitiative()
    {
        Unit_Master[] AllUnits = FindObjectsOfType<Unit_Master>();

        foreach (Unit_Master x in AllUnits)
        {
            x.RollInitiative();
        }

        AllUnits = AllUnits.OrderByDescending(x => x.characterSheet.UnitStat_Initiative).ToArray();

        initiativeOrder.Clear();

        foreach (Unit_Master x in AllUnits)
        {
            if (x.isDead != true && x.cantBeControlled == false)
            {
                initiativeOrder.Add(x);
            }
        }

        manager_HUD.hud_InitiativeOrder = initiativeOrder;
    }

    void AddAllUnitsToOverlordHordes()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            AddUnitToOverlordHorde(x);
        }
    }

    public void AddUnitToOverlordHorde(Unit_Master unitToAdd)
    {
        //Overlord_Master[] tempArray = FindObjectsOfType<Overlord_Master>();
        foreach (Overlord_AI x in allOverlordsInScene)
        {
            if (x.overlordFactionTag == unitToAdd.characterSheet.UnitStat_FactionTag)
            {
                x.AddUnitToHorde(unitToAdd);
            }
        }
    }

    //Remove all dead units from the scene
    void DestroyDeadUnits()
    {
        foreach (Unit_Master x in AllUnits)
        {
            if (x.isDead == true)
            {
                //Destroy(x.gameObject); probably dont need this anymore
                x.gameObject.SetActive(false);
            }
        }
    }

    //Select the first unit to take control of  // come back to this one it may be not needed !!
    void SelectTheFirstUnit()
    {
        selected_Unit = null;

        SelectedUnitIndex = 0;

        //selectedUnit = initiativeOrder[SelectedUnitIndex];
        SetSelectedOverlordAndUnit(SelectedUnitIndex);

        foreach (Unit_Master x in initiativeOrder)
        {
            if (x != selected_Unit)
            {
                //x.ToggleControl(false);
                DeactivateUnitToNotControl(x);
            }
        }
    }

    //Restore all movement points to all units and reset can move bool
    void ResetUnitMovement()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.ResetMovement();
        }
    }

    //Restore nerce of all units
    public void GiveAllUnitsNerve()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.RecoverNerve();
        }
    }

    public void StrategicModeToggle(bool toggle)
    {
        //new version of activateMapCam
        if (toggle)
        {
            DeactivateSuppressors();
            isInStrategicMode = true;
            //isInMapMode = true;
            AudioListener tempCamAudioListener = MapCamera.GetComponent<AudioListener>();
            tempCamAudioListener.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            MapCamera.targetTexture = null;

            manager_HUD.HUD_StrategicModeToggle(toggle);
        }

        //new version of deactivate map cam
        if (!toggle)
        {
            Cursor.lockState = CursorLockMode.Locked;
            AudioListener tempCamAudioListener = MapCamera.GetComponent<AudioListener>();
            tempCamAudioListener.enabled = false;
            MapCamera.targetTexture = (RenderTexture)manager_HUD.MiniMap.texture;
            //isInMapMode = false;
            isInStrategicMode = true;

            ActivateNewUnit();
        }
    }
    #endregion


    public void DeactivateSuppressors()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            if (x.Current_Unit_Suppression_State != Unit_Master.Unit_Suppression_States.State_Waiting)
            {
                x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_WaitingToSuppress;
            }
        }
    }

    //state 1a start round


    //state 1b act during their turn each round, complete objectives etc
    //state 1b2 when a unit is selected by the initiative order it's overlord is activated so
    //it can move, perform actions etc. If it is a player its player controlled, if ai, then ai etc


    //state 1c end round and start again 

    //Ends the unit's turn and calls FindNextActionsToActivate as a result
    public void EndUnitTurn()
    {
        //selectedUnit.Current_Unit_State = Unit_Master.Unit_States.State_Waiting;
        selected_Overlord.ccu.Current_Unit_State = Unit_Master.Unit_States.State_Waiting;
        //selectedUnit.CameraAudioListener.enabled = false;
        selected_Overlord.ccu.CameraAudioListener.enabled = false;
        

        TurnIsEnding = true;
        manager_HUD.MiniMapTransformReset();
        SelectNewUnitToActivate();
    }

    //check the win condition of the scenario
    public void CheckWinCondition()
    {
        //int PlayerUnits = 0;
        //int EnemyUnits = 0;

        //foreach (Unit_Master x in initiativeOrder)
        //{
        //    if (x.isDead == false)
        //    {
        //        if (x.characterSheet.UnitStat_FactionTag == Character_Master.FactionTag.SER)
        //        {
        //            PlayerUnits++;
        //        }

        //        if (x.characterSheet.UnitStat_FactionTag != Character_Master.FactionTag.SER)
        //        {
        //            EnemyUnits++;
        //        }
        //    }
        //}

        //if (PlayerUnits == 0 || EnemyUnits == 0)
        //{
        //    EndBattle();
        //}
    }

    //Selects next unit in the initiative order
    void SelectNewUnitToActivate()
    {
        SelectedUnitIndex++;

        if (SelectedUnitIndex >= initiativeOrder.Count)
        {
            EndRound();
        }

        else
        {
            if (selected_Unit != null)
            {
                //selectedUnit.ToggleControl(false);
                DeactivateUnitToNotControl(selected_Unit);
            }

            if (initiativeOrder[SelectedUnitIndex].isDead == false)
            {
                //selectedUnit = initiativeOrder[SelectedUnitIndex];
                SetSelectedOverlordAndUnit(SelectedUnitIndex);

                for (int i = 0; i < initiativeOrder.Count; i++)
                {
                    if (initiativeOrder[i] != selected_Unit)
                    {
                        if (initiativeOrder[i] != null)
                        {
                            //initiativeOrder[i].ToggleControl(false);
                            DeactivateUnitToNotControl(initiativeOrder[i]);
                        }
                    }
                }
                StrategicModeToggle(true);
            }

            else
            {
                SelectNewUnitToActivate();
            }
        }
    }

    void EndRound()
    {
        DestroyAllDeployables();
        ResetUnitStateMachines();
        manager_HUD.ResetNotificationFeed();
        StartRound();
    }

    public void DestroyAllDeployables()
    {
        Deployable_Object_Master[] deployablesToDelete = FindObjectsOfType<Deployable_Object_Master>();

        foreach (Deployable_Object_Master x in deployablesToDelete)
        {
            Destroy(x.gameObject);
        }
    }

    public void ResetUnitStateMachines()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            if (x != null)
            {
                x.StopAllCoroutines();
                x.isOnSuppressionCooldown = false;
                x.shooting.StopAllCoroutines();
                x.shooting.isFiring = false;
                x.Current_Unit_State = Unit_Master.Unit_States.State_Waiting;
                x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_Waiting;
            }
        }
    }

    public void ActivateSuppressors()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            if (x.Current_Unit_Suppression_State != Unit_Master.Unit_Suppression_States.State_Waiting)
            {
                if (x.suppressionTarget == selected_Unit)
                    x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_Suppressing;
                else
                    x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_WaitingToSuppress;
            }
        }
    }

    void ActivateNewUnit()
    {
        TurnIsEnding = false;
        //selectedUnit.ToggleControl(true);
        ActivateUnitToControl(selected_Unit);
        ActivateSuppressors();
    }

    public void ExitApplication()
    {
        Application.Quit();
    }  

    public void EndBattle()
    {
        manager_HUD.EndOfBattleCanvas.SetActive(true);
    }

    public void RechargeAllSuppressors(Unit_Master actingUnit)
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            if (x != actingUnit)
                x.RechargeSuppression();
        }
    }


    //maybe come back and change these to be a single scipt but the toggle functions are generally
    //unclear and can create easy to make mistakes with their ambiguity
    public void ActivateUnitToControl(Unit_Master unitToControl){ UnitActivationToggle(unitToControl, true); }
    public void DeactivateUnitToNotControl(Unit_Master unitToDeactivate) { UnitActivationToggle(unitToDeactivate, false); }
    public void UnitActivationToggle(Unit_Master targetUnit, bool toggle)
    {
        foreach (Overlord_Master x in allOverlordsInScene)
        {
            if (x.overlordFactionTag == targetUnit.characterSheet.UnitStat_FactionTag)
            {
                x.ToggleControlOfUnit(targetUnit, true);
            }

        }
    }
}
