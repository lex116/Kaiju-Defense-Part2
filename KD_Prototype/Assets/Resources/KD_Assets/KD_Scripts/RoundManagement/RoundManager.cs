using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    #region TeamColors
    [Header("Team Colors")]
    [SerializeField]
    public Material[] Skins;
    [SerializeField]
    public Sprite[] MapUnitIcons;
    public Color[] MapIconColors =
    {
        Color.blue,
        Color.yellow,
        Color.red
    };
    #endregion

    #region Notification Feed
    [Header("Notification Feed")]

    [SerializeField]
    public List<Text> KillFeedBoxes = new List<Text>();
    #endregion

    #region Map Hud Fields
    [Header("Map Hud Fields")]
    bool isInMapMode;
    public Camera MapCamera;

    public GameObject MapHUD;
    public Text HUD_Map_nameText;
    public Text HUD_Map_hpText;
    public Text HUD_Map_heldWeaponText;
    public Text HUD_Map_accText;
    [SerializeField]
    public List<Text> InitiatveOrderFeedBoxes = new List<Text>();
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
    public Text HUD_Player_ActionText;
    public GameObject HUD_Player_ConfirmText;

    public RawImage MiniMap;

    //-630
    internal float MiniMap_Default_posX;
    //280
    internal float MiniMap_Default_posY;
    //300
    internal float MiniMap_Default_Width;
    //300
    internal float MiniMap_Default_Height;

    internal float MiniMap_Target_PosX = 0;
    internal float MiniMap_Target_posY = 0;
    internal float MiniMap_Target_Width = 800;
    internal float MiniMap_Target_Height = 800;

    bool miniMapIsDefault = true;

    public float MapChangeRate = 10;
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

    #region Methods

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

        if (SelectedUnit != null)
        {
            if (SelectedUnit.isDead == true)
            {
                SelectedUnit = null;
                EndUnitTurn();
            }
        }

        ChangeMiniMapSize();
    }
    void PlayerHudUpdate()
    {
        HUD_Player_staminaBar.fillAmount = SelectedUnit.movementPointsRemaining / SelectedUnit.startingMovementPoints;

        HUD_Player_heldWeaponText.text = SelectedUnit.equippedWeapon.Item_Name;

        HUD_Player_hpText.text = "Hp : " + SelectedUnit.characterSheet.UnitStat_HitPoints;

        HUD_Player_nameText.text = SelectedUnit.characterSheet.UnitStat_Name;

        HUD_Player_accText.text = "Acc: " + SelectedUnit.Calculated_WeaponAccuracy + "%";

        HUD_Player_lookAtText.text = SelectedUnit.LookedAtObjectString;

        HUD_Player_ActionText.text = "" + SelectedUnit.SelectedAction;
    }

    //0. Calls StartBattle()
    void Start()
    {
        StartBattle();
        ClearNotificationFeed();
        ClearInitiativeOrderFeed();
    }

    //1. Starts the battle scenario, playing a cinematic or text etc, then calls StartRound()
    void StartBattle()
    {
        AssignTeamColors();
        SetMiniMapDefaults();

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
        GiveAllUnitsNerve();

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

        AllUnits = tempUnits.OrderByDescending(x => x.characterSheet.UnitStat_Initiative).ToArray();

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
        //SelectedUnit = null;
        MiniMapTransformReset();
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
            if (x.ActingUnit.isDead == false)
            {
                x.ActionEffect();

                while (x.ActingUnit.shooting.isFiring)
                {
                    yield return new WaitForFixedUpdate();
                }

                yield return new WaitForSeconds(1f);
            }
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

        //Debug.Log("unit index: " + SelectedUnitIndex);

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


    //Actitvate Map cam


    //Activate the unit to be controlled
    void ActivateNewUnit()
    {
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
                x.isAbleToSuppress = true;
            }
            else
            {
                x.isAbleToSuppress = false;
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
        ActivateRemainingActions();
    }
    //activate all remaining actions
    void ActivateRemainingActions()
    {
        StartCoroutine(ActivateRemainingActionsRoutine());
    }
    IEnumerator ActivateRemainingActionsRoutine()
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


        foreach (TimeScaleAction x in ActionsToActivate)
        {
            if (x.ActingUnit.isDead == false)
            {
                x.ActionEffect();

                while (x.ActingUnit.shooting.isFiring)
                {
                    yield return new WaitForFixedUpdate();
                }

                yield return new WaitForSeconds(1f);
            }
        }

        ActionsToActivate = null;

        int numberOfRemainingActions = 0;

        foreach (TimeScaleAction x in TimeScaleOrder)
        {
            if (x.timeScalePosition > CurrentTime)
            {
                numberOfRemainingActions++;
            }
        }

        if (numberOfRemainingActions > 0)
        {
            IncrememntTime();
            ActivateRemainingActions();
        }

        if (numberOfRemainingActions == 0)
        {
            ClearOldActions();
            ResetAllUnitStateMachines();

            AddNotificationToFeed("End of Round");

            StartRound();
        }
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
            //x.ShootingStateMachine.SetInteger("ShootingMode", 0);
            x.ShootingStateMachine.SetBool("isSPR", false);
            x.ShootingStateMachine.SetBool("Reset", true);
        }
    }
    #endregion

    public void ActivateMapCam()
    {
        DeactivateSuppressors();

        isInMapMode = true;
        Cursor.lockState = CursorLockMode.None;

        PlayerHUD.gameObject.SetActive(false);

        MapHUD.SetActive(true);
        MapCamera.targetTexture = null;

        HUD_Map_nameText.text = SelectedUnit.characterSheet.UnitStat_Name;
        HUD_Map_hpText.text = "Hp : " + SelectedUnit.characterSheet.UnitStat_HitPoints;
        HUD_Map_heldWeaponText.text = SelectedUnit.equippedWeapon.Item_Name;
        HUD_Map_accText.text = "Acc: " + SelectedUnit.Calculated_WeaponAccuracy + "%";

        ClearInitiativeOrderFeed();

        foreach (Unit_Master x in initiativeOrder)
        {
            InitiatveOrderFeedBoxes[initiativeOrder.IndexOf(x)].text = x.characterSheet.UnitStat_Name;
        }
    }

    public void DeactivateMapCam()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInMapMode = false;

        PlayerHUD.gameObject.SetActive(true);

        MapHUD.SetActive(false);
        MapCamera.targetTexture = (RenderTexture)MiniMap.texture;

        HUD_Player_ConfirmText.SetActive(false);

        ActivateNewUnit();
    }

    public void DeactivateSuppressors()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.isAbleToSuppress = false;
        }
    }

    #endregion

    public void AddNotificationToFeed(string note)
    {
        foreach (Text x in KillFeedBoxes)
        {
            if (KillFeedBoxes.IndexOf(x) <= KillFeedBoxes.Count - 2)
            {
                x.text = KillFeedBoxes[KillFeedBoxes.IndexOf(x) + 1].text;
            }
        }

        KillFeedBoxes[KillFeedBoxes.Count - 1].text = note;
    }

    public void ClearNotificationFeed()
    {
        foreach (Text x in KillFeedBoxes)
        {
            x.text = "";
        }
    }

    public void ClearInitiativeOrderFeed()
    {
        foreach (Text x in InitiatveOrderFeedBoxes)
        {
            x.text = "";
        }
    }

    public void GiveAllUnitsNerve()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.ChangeNerve(5);
        }
    }

    public void AssignTeamColors()
    {
        Unit_Master[] UnitsToColor = FindObjectsOfType<Unit_Master>();

        foreach (Unit_Master x in UnitsToColor)
        {
            foreach (MeshRenderer y in x.UnitSkins)
            {
                y.material = Skins[(int)x.characterSheet.UnitStat_FactionTag];
            }

            x.MapIconHighlight.color = MapIconColors[(int)x.characterSheet.UnitStat_FactionTag];

            x.UnitIcon.sprite = MapUnitIcons[(int)x.characterSheet.unitType];
        }
    }

    public void SetMiniMapDefaults()
    {
        MiniMap_Default_posX = MiniMap.rectTransform.anchoredPosition.x;
        MiniMap_Default_posY = MiniMap.rectTransform.anchoredPosition.y;
        MiniMap_Default_Width = MiniMap.rectTransform.sizeDelta.x;
        MiniMap_Default_Height = MiniMap.rectTransform.sizeDelta.y;
        miniMapIsDefault = true;
    }

    public void ToggleMiniMap()
    {
        if (miniMapIsDefault == false)
        {
            miniMapIsDefault = true;
        }

        else if (miniMapIsDefault == true)
        {
            miniMapIsDefault = false;
        }
    }

    public void ChangeMiniMapSize()
    {
        if (miniMapIsDefault)
        {
            #region return to default 

            if (MiniMap.rectTransform.anchoredPosition.x > MiniMap_Default_posX 
                && MiniMap.rectTransform.anchoredPosition.y < MiniMap_Default_posY)
            {
                MiniMap.rectTransform.anchoredPosition = 
                    new Vector3(MiniMap.rectTransform.anchoredPosition.x - MapChangeRate, 
                    MiniMap.rectTransform.anchoredPosition.y + MapChangeRate, 0);
            }

            if (MiniMap.rectTransform.anchoredPosition.x > MiniMap_Default_posX)
            {
                MiniMap.rectTransform.anchoredPosition =
                    new Vector3(MiniMap.rectTransform.anchoredPosition.x - MapChangeRate, 
                    MiniMap.rectTransform.anchoredPosition.y, 0);
            }

            if (MiniMap.rectTransform.anchoredPosition.y < MiniMap_Default_posY)
            {
                MiniMap.rectTransform.anchoredPosition =
                    new Vector3(MiniMap.rectTransform.anchoredPosition.x, 
                    MiniMap.rectTransform.anchoredPosition.y + MapChangeRate, 0);
            }


            if (MiniMap.rectTransform.sizeDelta.x > MiniMap_Default_Width
                && MiniMap.rectTransform.sizeDelta.y > MiniMap_Default_Height)
            {
                MiniMap.rectTransform.sizeDelta =
                    new Vector3(MiniMap.rectTransform.sizeDelta.x - MapChangeRate, MiniMap.rectTransform.sizeDelta.y - MapChangeRate, 0);
            }

            #endregion
        }

        if (!miniMapIsDefault)
        {
            #region go to target 

            if (MiniMap.rectTransform.anchoredPosition.x < MiniMap_Target_PosX
                && MiniMap.rectTransform.anchoredPosition.y > MiniMap_Target_posY)
            {
                MiniMap.rectTransform.anchoredPosition =
                    new Vector3(MiniMap.rectTransform.anchoredPosition.x + MapChangeRate,
                    MiniMap.rectTransform.anchoredPosition.y - MapChangeRate, 0);
            }

            if (MiniMap.rectTransform.anchoredPosition.x < MiniMap_Target_PosX)
            {
                MiniMap.rectTransform.anchoredPosition =
                    new Vector3(MiniMap.rectTransform.anchoredPosition.x + MapChangeRate,
                    MiniMap.rectTransform.anchoredPosition.y, 0);
            }

            if (MiniMap.rectTransform.anchoredPosition.y > MiniMap_Target_posY)
            {
                MiniMap.rectTransform.anchoredPosition =
                    new Vector3(MiniMap.rectTransform.anchoredPosition.x,
                    MiniMap.rectTransform.anchoredPosition.y - MapChangeRate, 0);
            }


            if (MiniMap.rectTransform.sizeDelta.x < MiniMap_Target_Width
                && MiniMap.rectTransform.sizeDelta.y < MiniMap_Target_Height)
            {
                MiniMap.rectTransform.sizeDelta =
                    new Vector3(MiniMap.rectTransform.sizeDelta.x + MapChangeRate, MiniMap.rectTransform.sizeDelta.y + MapChangeRate, 0);
            }

            #endregion
        }
    }

    public void MiniMapTransformReset()
    {
        miniMapIsDefault = true;
        MiniMap.rectTransform.anchoredPosition = new Vector3(MiniMap_Default_posX, MiniMap_Default_posY, 0);
        MiniMap.rectTransform.sizeDelta = new Vector2(MiniMap_Default_Width, MiniMap_Default_Height);
    }
}