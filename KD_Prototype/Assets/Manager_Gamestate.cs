using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager_GameState : MonoBehaviour
{
    #region setup fields
    //scenario classS
    public Blueprint_Scenario_Master selectedScenario;
    //spawn points in the scenario
    public SpawnPoint[] playerSpawnPointsInScenario;
    public SpawnPoint[] enemySpawnPointsInScenario;
    //all overlords in the scenario
    public List<Overlord_Master> allOverlordsInScene = new List<Overlord_Master>();
    //all units in the scenario
    public List<Unit_Master> allUnitsInScene = new List<Unit_Master>();
    //hud manager
    Manager_HUD manager_HUD;
    //file path strings
    internal string M_HUD_Obj_String = "Manager_HUD_Object";
    internal string O_P_Obj_String = "Overlord_Player_Object";
    internal string O_AI_Obj_String = "Overlord_AI_Object";
    #endregion

    #region during round fields
    //unit and overlord who will act next
    public Unit_Master selected_Unit;
    public Overlord_Master selected_Overlord;
    //all units in the scene
    internal Unit_Master[] AllUnits;
    //index for the initiative order (helps select unit)
    public int SelectedUnitIndex;
    //the initoative order, what order units take their turns
    [SerializeField]
    List<Unit_Master> initiativeOrder = new List<Unit_Master>();
    //if the unit id ending it's turn
    public bool TurnIsEnding;
    //in overhead view or in fps view
    public bool isInStrategicMode;
    #endregion

    //0 on awake lock frame rate and cursor
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        //turn this back on when we have a playable fucking game again
        //Cursor.lockState = CursorLockMode.Locked;
        manager_HUD = Instantiate(Resources.Load<GameObject>(KD_Global.filePath_Prefab + M_HUD_Obj_String)).GetComponent<Manager_HUD>();
        manager_HUD.activateUnitButton.onClick.AddListener(() => StrategicModeToggle(false));
        selectedScenario = (Blueprint_Scenario_Master)ScriptableObject.CreateInstance<Blueprint_Scenario_1_TestMap>();
        SetBattleScenario();
    }

    #region //1 set scenario setup
    //void SetBattleScenario(Blueprint_Scenario_Master scenario)
    void SetBattleScenario()
    {
        Overlord_Master tempOverlordReference;
        //read scenario blueprint from selected scenario class
        //selectedScenario = scenario;

        //instantiate map
        Instantiate(Resources.Load<GameObject>(KD_Global.filePath_Prefab + selectedScenario.scenarioMap_Name));

        //get all spawn locations in our scenario's map
        SpawnPoint[] tempAllspawns = FindObjectsOfType<SpawnPoint>();
        List<SpawnPoint> tempPlayerSpawns = new List<SpawnPoint>();
        List<SpawnPoint> tempEnemySpawns = new List<SpawnPoint>();
        foreach (SpawnPoint x in tempAllspawns)
        {
            if (x.spawnOwner == SpawnPoint.owner.player)
            {
                tempPlayerSpawns.Add(x);
            }

            if (x.spawnOwner == SpawnPoint.owner.enemy)
            {
                tempEnemySpawns.Add(x);
            }
        }        
        playerSpawnPointsInScenario = tempPlayerSpawns.OrderBy(y => y.spawnID).ToArray();
        enemySpawnPointsInScenario = tempEnemySpawns.OrderBy(z => z.spawnID).ToArray();

        //generate a number of overlords for each faction in the scenario
        tempOverlordReference = 
            Instantiate(Resources.Load<GameObject>(KD_Global.filePath_Prefab + O_P_Obj_String)).GetComponent<Overlord_Master>();
        tempOverlordReference.overlordFactionTag = selectedScenario.playerOverlordFaction;
        tempOverlordReference.SetManagers(this, manager_HUD);
        allOverlordsInScene.Add(tempOverlordReference);

        tempOverlordReference = 
            Instantiate(Resources.Load<GameObject>(KD_Global.filePath_Prefab + O_AI_Obj_String)).GetComponent<Overlord_Master>();
        tempOverlordReference.overlordFactionTag = selectedScenario.aiOverlordFaction;
        tempOverlordReference.SetManagers(this, manager_HUD);
        allOverlordsInScene.Add(tempOverlordReference);

        //generate units from unit blueprints from the scenario class
        for (int i = 0; i < selectedScenario.playerBlueprintUnits.Count; i++)
        {
            Blueprint_Unit_Master tempBU = null;
            tempBU = 
                (Blueprint_Unit_Master)ScriptableObject.CreateInstance(selectedScenario.playerBlueprintUnits[i].ToString());
            KD_Global.SpawnUnit(tempBU, playerSpawnPointsInScenario[i]);
        }
        for (int i = 0; i < selectedScenario.enemyBlueprintUnits.Count; i++)
        {
            Blueprint_Unit_Master tempBU = null;
            tempBU =
                (Blueprint_Unit_Master)ScriptableObject.CreateInstance(selectedScenario.enemyBlueprintUnits[i].ToString());
            KD_Global.SpawnUnit(tempBU, enemySpawnPointsInScenario[i]);
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
    #endregion

    #region //2 start a round
    //Order all units in the scene by initiative, remove dead units, select the first unit
    void StartRound()
    {
        OrderUnitsByInitiative();
        DestroyDeadUnits();
        AddAllUnitsToOverlordHordes();
        SelectTheFirstUnit();
        ResetUnitMovement();
        GiveAllUnitsNerve();
        StrategicModeToggle(true);
    }
    //Find all units in the scene, order them by initiative ignoring the dead units
    //also give this to the hud manager so we don't have to reference it a thousand times
    void OrderUnitsByInitiative()
    {
        AllUnits = FindObjectsOfType<Unit_Master>();

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
    // Assigns all units to their overlord
    void AddAllUnitsToOverlordHordes()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            AddUnitToOverlordHorde(x);
        }
    } 
    public void AddUnitToOverlordHorde(Unit_Master unitToAdd)
    {
        foreach (Overlord_Master x in allOverlordsInScene)
        {
            if (x.overlordFactionTag == unitToAdd.characterSheet.UnitStat_FactionTag)
            {
                x.AddUnitToHorde(unitToAdd);
            }
        }
    }
    //Select the first unit to take control of  // come back to this one it may be not needed !!
    void SelectTheFirstUnit()
    {
        selected_Unit = null;
        SelectedUnitIndex = 0;

        SetSelectedOverlordAndUnit(SelectedUnitIndex);

        foreach (Unit_Master x in initiativeOrder)
        {
            //if (x != selected_Unit)
            //{
                DeactivateUnitToNotControl(x);
            //}
        }
    }
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
    //maybe come back and change these to be a single scipt but the toggle functions are generally
    //unclear and can create easy to make mistakes with their ambiguity
    public void ActivateUnitToControl(Unit_Master unitToControl) { UnitActivationToggle(unitToControl, true); }
    public void DeactivateUnitToNotControl(Unit_Master unitToDeactivate) { UnitActivationToggle(unitToDeactivate, false); }
    public void UnitActivationToggle(Unit_Master targetUnit, bool toggle)
    {
        foreach (Overlord_Master x in allOverlordsInScene)
        {
            if (x.overlordFactionTag == targetUnit.characterSheet.UnitStat_FactionTag)
            {
                x.ToggleControlOfUnit(targetUnit, toggle);
            }
        }
    }
    //Restore all movement points to all units and reset cantMove bool
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
    //toggles between the map and control of a unit
    public void StrategicModeToggle(bool toggle)
    {
        //new version of activateMapCam
        if (toggle)
        {
            isInStrategicMode = true;
            Cursor.lockState = CursorLockMode.None;
            DeactivateSuppressors();
            AudioListener tempCamAudioListener = manager_HUD.mapCamera.GetComponent<AudioListener>();
            tempCamAudioListener.enabled = true;
            manager_HUD.mapCamera.targetTexture = null;
            manager_HUD.HUD_StrategicModeToggle(toggle);
        }
        //new version of deactivate map cam
        if (!toggle)
        {
            isInStrategicMode = false;
            Cursor.lockState = CursorLockMode.Locked;
            AudioListener tempCamAudioListener = manager_HUD.mapCamera.GetComponent<AudioListener>();
            tempCamAudioListener.enabled = false;
            manager_HUD.mapCamera.targetTexture = (RenderTexture)manager_HUD.MiniMap.texture;
            manager_HUD.HUD_StrategicModeToggle(toggle);
            ActivateNewUnit();
        }
    }
    //tells all suppressing units to stop preparing to fire and go back to waiting to suppress
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
    //activates the selected unit so it can take its turn
    void ActivateNewUnit()
    {
        TurnIsEnding = false;
        ActivateUnitToControl(selected_Unit);
        ActivateSuppressors();
    }
    // tells all the units waiting to suppress to change to waiting to fire if their target is the selected unit
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
    #endregion

    #region //3 round ongoing
    //Ends the unit's turn and selects a new unit 
    public void EndUnitTurn()
    {
        selected_Overlord.ccu.Current_Unit_State = Unit_Master.Unit_States.State_Waiting;
        selected_Overlord.ccu.CameraAudioListener.enabled = false;
        TurnIsEnding = true;
        manager_HUD.MiniMapTransformReset();
        SelectNewUnitToActivate();
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
                DeactivateUnitToNotControl(selected_Unit);
            }

            if (initiativeOrder[SelectedUnitIndex].isDead == false)
            {
                SetSelectedOverlordAndUnit(SelectedUnitIndex);

                for (int i = 0; i < initiativeOrder.Count; i++)
                {
                    if (initiativeOrder[i] != selected_Unit)
                    {
                        if (initiativeOrder[i] != null)
                        {
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
    //Not called by manager, when a suppressed unit acts call this
    public void RechargeAllSuppressors(Unit_Master actingUnit)
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            if (x != actingUnit)
                x.RechargeSuppression();
        }
    }
    #endregion

    #region //4 end round functions
    //clean up, reset things, and start the new round
    void EndRound()
    {
        DestroyAllDeployables();
        ResetUnitStateMachines();
        manager_HUD.ResetNotificationFeed();
        StartRound();
    }
    //destroys all deployables
    public void DestroyAllDeployables()
    {
        Deployable_Object_Master[] deployablesToDelete = FindObjectsOfType<Deployable_Object_Master>();
        foreach (Deployable_Object_Master x in deployablesToDelete)
        {
            Destroy(x.gameObject);
        }
    }
    //stop shooting, stop suppressing, reset both
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
    #endregion

    #region //x random shit
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
    //pulls up the end battle *stuff* might add more later
    public void EndBattle()
    {
        manager_HUD.EndOfBattleCanvas.SetActive(true);
    }
    // quits the game
    public void ExitApplication()
    {
        Application.Quit();
    }
    //1 Update the hud as it changes //2 Check if the playe is dead
    //3 update the minimap size //4 check for the win condition of the scenario
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
    #endregion
}