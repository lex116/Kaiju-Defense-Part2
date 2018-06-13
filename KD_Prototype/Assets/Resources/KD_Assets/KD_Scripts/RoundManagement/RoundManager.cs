using System; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public GameObject EndOfBattleCanvas;

    public Image Reticle;

    public GameObject Player_HUD_Basic;
    public GameObject Player_HUD_Moving;
    public GameObject Player_HUD_Action;
    public GameObject Player_HUD_Shooting;
    public GameObject Player_HUD_Equipment;

    internal Color32 ActionIcon_Selected_Opacity = new Color32 (255, 255, 255, 255);
    internal Color32 ActionIcon_NotSelected_Opacity = new Color32 (255, 255, 255, 50);

    [SerializeField]
    List<Image> ActionTray = new List<Image>();

    enum Manager_States
    {
        State_StartingBattle,
        State_StartingRound,
        State_MapOverview,
        State_InMenu,
        State_UnitActing,
        State_EndingRound,
        State_EndingBattle
    }

    Manager_States CurrentState;

    public GameObject InitiativeOrderBar;

    public Text ProbableAmountOfDamageToDeal_Text;

    public GameObject ExitApplicationPanel;

    public Text RemainingAP;

    #region TeamColors
    [Header("Team Colors")]
    [SerializeField]
    public Material[] Skins;
    [SerializeField]
    public Sprite[] MapUnitIcons;
    internal Color[] MapIconColors =
    {
        Color.blue,
        Color.yellow,
        Color.red,
        Color.white
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
    internal float HUD_Player_UpdateRate = .02f;

    public GameObject PlayerHUD;
    public GameObject PlayerTargetHUD;

    public Image HUD_Player_staminaBar;
    public Image HUD_Player_healthBar;
    public Image HUD_Player_nerveBar;
    public Text HUD_Player_hpText;
    public Text HUD_Player_staminaText;
    public Text HUD_Player_nerveText;
    public Text HUD_Player_nameText;
    public Text HUD_Player_heldWeaponText;
    public Text HUD_Player_heldEquipmentText;
    public Text HUD_Player_accText;
    public Text HUD_Player_ActionText;
    //Temp
    public Text HUD_Player_StateText;
    public GameObject HUD_Player_ConfirmText;

    public Text HUD_Player_Target_nameText;
    public Image HUD_Player_Target_healthBar;
    public Image HUD_Player_Target_nerveBar;
    public Text HUD_Player_Target_hpText;
    public Text HUD_Player_Target_nerveText;

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
    internal float MiniMap_Target_Width = 900;
    internal float MiniMap_Target_Height = 900;

    bool miniMapIsDefault = true;

    internal float MapChangeRate = 50;

    internal float HUD_Player_DisplayAcc;
    internal float HUD_Player_TargetAcc;
    internal float HUD_Player_DisplayHp;
    internal float HUD_Player_TargetHp;
    internal float HUD_Player_DisplayNerve;
    internal float HUD_Player_TargetNerve;

    internal float HUD_Player_Target_DisplayHp;
    internal float HUD_Player_Target_TargetHp;
    internal float HUD_Player_Target_DisplayNerve;
    internal float HUD_Player_Target_TargetNerve;
    #endregion

    #region InitiativeFields
    public Unit_Master SelectedUnit;
    //public Unit_Human SelectedUnit;
    public Unit_Master[] AllUnits;
    public int SelectedUnitIndex;
    [SerializeField]
    List<Unit_Master> initiativeOrder = new List<Unit_Master>();
    public bool TurnIsEnding;
    #endregion

    //

    #region Utility Methods
    // Mouse Locking
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Cursor.lockState = CursorLockMode.Locked;
        ExitApplicationPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Cursor.lockState != CursorLockMode.Locked && isInMapMode == false && ExitApplicationPanel.activeSelf == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (SelectedUnit != null && isInMapMode == false)
        {
            PlayerHudUpdate();
        }

        if (SelectedUnit != null && SelectedUnit.isDead == true && TurnIsEnding == false)
        {
            EndUnitTurn();
        }

        ChangeMiniMapSize();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitApplicationCanvas();
        }

        CheckWinCondition();
    }
    #endregion

    #region Hud Methods

    void PlayerHudUpdate()
    {
        HUD_Player_heldWeaponText.text = SelectedUnit.equippedWeapon.Item_Name;

        if (SelectedUnit is Unit_VehicleMaster)
        {
            Unit_VehicleMaster x = (Unit_VehicleMaster)SelectedUnit;

            if (x.CurrentPilot_Character != null)
            HUD_Player_nameText.text = x.characterSheet.UnitStat_Name + "(" + x.CurrentPilot_Character.UnitStat_Name + ")";
        }

        HUD_Player_heldEquipmentText.text = SelectedUnit.equippedEquipment.Item_Name + ": " + SelectedUnit.equippedEquipment.Ammo;

        HUD_Player_nameText.text = SelectedUnit.characterSheet.UnitStat_Name;

        HUD_Player_ActionText.text = "" + SelectedUnit.Selected_Unit_Action.Action_Name;
        HUD_Player_StateText.text = "" + SelectedUnit.Current_Unit_State;

        PlayerStatUpdates();

        ToggleTargetHUD();

        RemainingAP.text = "AP: " + SelectedUnit.AP + " -> " + (SelectedUnit.AP - SelectedUnit.Selected_Unit_Action.Action_AP_Cost);
    }

    public void ToggleTargetHUD()
    {
        if (SelectedUnit.LookedAtUnit_Master != null || SelectedUnit.LookedAtUnit_VehicleHardPoint != null)
        {
            PlayerTargetHUD.SetActive(true);
            TargetStatUpdates();
        }

        else
        {
            PlayerTargetHUD.SetActive(false);
            ResetTargetHUD();
        }
    }

    #region PlayerStatUpdates
    void PlayerStatUpdates()
    {
        UpdatePlayerStamina();
        UpdatePlayerNerve();
        UpdatePlayerHp();
        UpdatePlayerAcc();

        UpdatePlayerActions();
    }

    void UpdatePlayerStamina()
    {
        HUD_Player_staminaBar.fillAmount = SelectedUnit.movementPointsRemaining / SelectedUnit.startingMovementPoints;

        HUD_Player_staminaText.text = ((int)SelectedUnit.movementPointsRemaining).ToString();
    }
    void UpdatePlayerNerve()
    {
        float currentNerve = 0;
        float startingNerve = 0;

        if (SelectedUnit.characterSheet.unitType == Character_Master.UnitTypes.Human)
        {
            currentNerve = SelectedUnit.characterSheet.UnitStat_Nerve;
            startingNerve = SelectedUnit.characterSheet.UnitStat_StartingNerve;

            HUD_Player_nerveText.text = SelectedUnit.characterSheet.UnitStat_Nerve.ToString();
        }

        if (SelectedUnit.characterSheet.unitType == Character_Master.UnitTypes.Vehicle ||
            SelectedUnit.characterSheet.unitType == Character_Master.UnitTypes.Mecha)
        {
            Unit_VehicleMaster SelectedVehicle = (Unit_VehicleMaster)SelectedUnit;
            if (SelectedVehicle.CurrentPilot_Character != null)
            {
                currentNerve = SelectedVehicle.CurrentPilot_Character.UnitStat_Nerve;
                startingNerve = SelectedVehicle.CurrentPilot_Character.UnitStat_StartingNerve;
                HUD_Player_nerveText.text = SelectedVehicle.CurrentPilot_Character.UnitStat_Nerve.ToString();
            }
        }

        HUD_Player_TargetNerve = (currentNerve / startingNerve);

        if (HUD_Player_DisplayNerve > HUD_Player_TargetNerve)
            HUD_Player_DisplayNerve = HUD_Player_DisplayNerve - HUD_Player_UpdateRate;

        if (HUD_Player_DisplayNerve < HUD_Player_TargetNerve)
            HUD_Player_DisplayNerve = HUD_Player_DisplayNerve + HUD_Player_UpdateRate;

        HUD_Player_nerveBar.fillAmount = HUD_Player_DisplayNerve;
    }
    void UpdatePlayerHp()
    {
        float currentHp = SelectedUnit.characterSheet.UnitStat_HitPoints;
        float startingHp = SelectedUnit.characterSheet.UnitStat_StartingHitPoints;

        HUD_Player_TargetHp = (currentHp / startingHp);

        if (HUD_Player_DisplayHp > HUD_Player_TargetHp)
            HUD_Player_DisplayHp = HUD_Player_DisplayHp - HUD_Player_UpdateRate;

        if (HUD_Player_DisplayHp < HUD_Player_TargetHp)
            HUD_Player_DisplayHp = HUD_Player_DisplayHp + HUD_Player_UpdateRate;

        HUD_Player_healthBar.fillAmount = HUD_Player_DisplayHp;

        HUD_Player_hpText.text = SelectedUnit.characterSheet.UnitStat_HitPoints.ToString();
    }
    void UpdatePlayerAcc()
    {
        HUD_Player_TargetAcc = SelectedUnit.Calculated_WeaponAccuracy * SelectedUnit.CurrentShotAccuracyModifier;

        if (HUD_Player_DisplayAcc > HUD_Player_TargetAcc)
            HUD_Player_DisplayAcc--;

        if (HUD_Player_DisplayAcc < HUD_Player_TargetAcc)
            HUD_Player_DisplayAcc++;

        HUD_Player_accText.text = (int)HUD_Player_DisplayAcc + "%";
    }

    void UpdatePlayerActions()
    {
        for (int i = 0; i < ActionTray.Count; i++)
        {
            ActionTray[i].sprite = SelectedUnit.Unit_Actions[i+1].Action_Icon;

            if (i + 1 == Array.IndexOf(SelectedUnit.Unit_Actions, SelectedUnit.Selected_Unit_Action))
                ActionTray[i].color = ActionIcon_Selected_Opacity;

            else
                ActionTray[i].color = ActionIcon_NotSelected_Opacity;
        }
    }

    #endregion

    #region TargetStatUpdates
    void TargetStatUpdates()
    {
        UpdatePlayerTargetProbableDamageToDeal();
        UpdatePlayerTargetName();
        UpdatePlayerTargetHp();
        UpdatePlayerTargetNerve();
    }

    void UpdatePlayerTargetProbableDamageToDeal()
    {
        int DmgToDealWithResist = 0;

        if (SelectedUnit.LookedAtUnit_Master != null)
            DmgToDealWithResist = SelectedUnit.equippedWeapon.Damage - SelectedUnit.LookedAtUnit_Master.equippedArmor.DamageResistance[(int)SelectedUnit.equippedWeapon.damageType];

        if (SelectedUnit.LookedAtUnit_VehicleHardPoint != null)
            DmgToDealWithResist = SelectedUnit.equippedWeapon.Damage - SelectedUnit.LookedAtUnit_VehicleHardPoint.AttachedArmor.DamageResistance[(int)SelectedUnit.equippedWeapon.damageType];

        int ProbableNumberOfShotsToHit = (int)Math.Ceiling(((SelectedUnit.equippedWeapon.ShotCount * SelectedUnit.equippedWeapon.BurstCount) * ((SelectedUnit.Calculated_WeaponAccuracy * SelectedUnit.CurrentShotAccuracyModifier) / 100)));

        int DmgToDealTimesShotCount = DmgToDealWithResist * ProbableNumberOfShotsToHit;

        if (DmgToDealTimesShotCount < 0)
            DmgToDealTimesShotCount = 0;

        ProbableAmountOfDamageToDeal_Text.text = "Avg Dmg: " + (int)DmgToDealTimesShotCount + " (" + DmgToDealWithResist + " DMG x " + ProbableNumberOfShotsToHit + " Shots)";
    }
    void UpdatePlayerTargetName()
    {
        if (SelectedUnit.LookedAtUnit_Master != null)
            HUD_Player_Target_nameText.text = SelectedUnit.LookedAtUnit_Master.characterSheet.UnitStat_Name;

        if (SelectedUnit.LookedAtUnit_VehicleHardPoint != null)
            HUD_Player_Target_nameText.text = SelectedUnit.LookedAtUnit_VehicleHardPoint.HardPointName;
    }
    void UpdatePlayerTargetHp()
    {
        float currentHp = 0;
        float startingHp = 0;

        if (SelectedUnit.LookedAtUnit_Master != null)
        {
            currentHp = SelectedUnit.LookedAtUnit_Master.characterSheet.UnitStat_HitPoints;
            startingHp = SelectedUnit.LookedAtUnit_Master.characterSheet.UnitStat_StartingHitPoints;

            HUD_Player_Target_hpText.text = SelectedUnit.LookedAtUnit_Master.characterSheet.UnitStat_HitPoints.ToString();
        }

        if (SelectedUnit.LookedAtUnit_VehicleHardPoint != null)
        {
            currentHp = SelectedUnit.LookedAtUnit_VehicleHardPoint.HitPoints;
            startingHp = SelectedUnit.LookedAtUnit_VehicleHardPoint.StartingHitPoints;

            HUD_Player_Target_hpText.text = SelectedUnit.LookedAtUnit_VehicleHardPoint.HitPoints.ToString();
        }

        HUD_Player_Target_TargetHp = (currentHp / startingHp);

        if (HUD_Player_Target_DisplayHp > HUD_Player_Target_TargetHp)
            HUD_Player_Target_DisplayHp = HUD_Player_Target_DisplayHp - HUD_Player_UpdateRate;
        
        if (HUD_Player_Target_DisplayHp < HUD_Player_Target_TargetHp)
            HUD_Player_Target_DisplayHp = HUD_Player_Target_DisplayHp + HUD_Player_UpdateRate;

        HUD_Player_Target_healthBar.fillAmount = HUD_Player_Target_DisplayHp;
    }
    void UpdatePlayerTargetNerve()
    {
        float currentNerve = 0;
        float startingNerve = 0;

        if (SelectedUnit.LookedAtUnit_Master != null)
        {
            currentNerve = SelectedUnit.LookedAtUnit_Master.characterSheet.UnitStat_Nerve;
            startingNerve = SelectedUnit.LookedAtUnit_Master.characterSheet.UnitStat_StartingNerve;

            HUD_Player_Target_nerveText.text = SelectedUnit.LookedAtUnit_Master.characterSheet.UnitStat_Nerve.ToString();
        }

        if (SelectedUnit.LookedAtUnit_VehicleHardPoint != null)
        {

            if (SelectedUnit.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character != null)
            {
                currentNerve = SelectedUnit.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character.UnitStat_Nerve;
                startingNerve = SelectedUnit.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character.UnitStat_StartingNerve;

                HUD_Player_Target_nerveText.text = SelectedUnit.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character.UnitStat_Nerve.ToString();
            }

            else
            {
                currentNerve = 0;
                startingNerve = 0;

                HUD_Player_Target_nerveText.text = "0";
            }

        }

        HUD_Player_Target_TargetNerve = (currentNerve / startingNerve);

        if (HUD_Player_Target_DisplayNerve > HUD_Player_Target_TargetNerve)
            HUD_Player_Target_DisplayNerve = HUD_Player_Target_DisplayNerve - HUD_Player_UpdateRate;

        if (HUD_Player_Target_DisplayNerve < HUD_Player_Target_TargetNerve)
            HUD_Player_Target_DisplayNerve = HUD_Player_Target_DisplayNerve + HUD_Player_UpdateRate;

        HUD_Player_Target_nerveBar.fillAmount = HUD_Player_Target_DisplayNerve;
    }
    #endregion

    void ResetPlayerHUD()
    {
        HUD_Player_DisplayAcc = 0;
        HUD_Player_TargetAcc = 0;
        HUD_Player_DisplayHp = 0;
        HUD_Player_TargetHp = 0;
        HUD_Player_DisplayNerve = 0;
        HUD_Player_TargetNerve = 0;
    }

    void ResetTargetHUD()
    {
        HUD_Player_Target_DisplayHp = 0;
        HUD_Player_Target_TargetHp = 0;
        HUD_Player_Target_DisplayNerve = 0;
        HUD_Player_Target_TargetNerve = 0;
    }

    public void AssignTeamColors(Unit_Master UnitToColor)
    {
        foreach (MeshRenderer x in UnitToColor.UnitSkins)
        {
            x.material = Skins[(int)UnitToColor.characterSheet.UnitStat_FactionTag];
        }

        UnitToColor.MapIconHighlight.color = MapIconColors[(int)UnitToColor.characterSheet.UnitStat_FactionTag];
        UnitToColor.UnitIcon.sprite = MapUnitIcons[(int)UnitToColor.characterSheet.unitType];
    }

    public void ToggleMiniMap()
    {
        if (miniMapIsDefault == false)
        {
            miniMapIsDefault = true;
            InitiativeOrderBar.SetActive(false);
            Reticle.gameObject.SetActive(true);
        }

        else if (miniMapIsDefault == true)
        {
            miniMapIsDefault = false;
            InitiativeOrderBar.SetActive(true);
            Reticle.gameObject.SetActive(false);
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
    #endregion

    #region Phase 0 - StartBattle

    void Start()
    {
        StartBattle();
        ClearKillFeed();
        ClearInitiativeFeed();
    }

    public void ClearKillFeed()
    {
        foreach (Text x in KillFeedBoxes)
        {
            x.text = "";
        }
    }
    public void ClearInitiativeFeed()
    {
        foreach (Text x in InitiatveOrderFeedBoxes)
        {
            x.text = "";
        }
    }

    void StartBattle()
    {
        AssignAllTeamColors();
        SetMiniMapDefaults();
        StartRound();
    }

    public void AssignAllTeamColors()
    {
        Unit_Master[] UnitsToColor = FindObjectsOfType<Unit_Master>();

        foreach (Unit_Master x in UnitsToColor)
        {
            AssignTeamColors(x);
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
    #endregion

    #region Phase 1 - StartRound
    //Order all units in the scene by initiative, remove dead units, select the first unit
    void StartRound()
    {
        OrderUnitsByInitiative();
        DestroyDeadUnits();
        SelectTheFirstUnit();
        ResetUnitMovement();
        GiveAllUnitsNerve();
        ActivateMapCam();
    }

    //Find all units in the scene, order them by initiative ignoring the dead units
    void OrderUnitsByInitiative()
    {
        AllUnits = null;

        Unit_Master[] tempUnits;

        tempUnits = null;

        tempUnits = FindObjectsOfType<Unit_Master>();

        foreach (Unit_Master x in tempUnits)
        {
            x.RollInitiative();
        }

        AllUnits = tempUnits.OrderByDescending(x => x.characterSheet.UnitStat_Initiative).ToArray();

        initiativeOrder.Clear();

        foreach (Unit_Master x in AllUnits)
        {
            if (x.isDead != true && x.cantBeControlled == false)
            {
                initiativeOrder.Add(x);
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
                Destroy(x.gameObject);
            }
        }
    }

    //Select the first unit to take control of
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
    }

    //Restore all movement points to all units and reset can move bool
    void ResetUnitMovement()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.ResetMovement();
        }
    }

    public void GiveAllUnitsNerve()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.RecoverNerve();
        }
    }
    #endregion

    #region Phase 2 - MapOverview
    public void ActivateMapCam()
    {
        DeactivateSuppressors();

        isInMapMode = true;

        AudioListener tempCamAudioListener = MapCamera.GetComponent<AudioListener>();
        tempCamAudioListener.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        ResetPlayerHUD();
        PlayerHUD.gameObject.SetActive(false);
        MapHUD.SetActive(true);
        InitiativeOrderBar.SetActive(true);
        MapCamera.targetTexture = null;

        ClearInitiativeFeed();

        StartCoroutine(WaitToSetMapHud());
    }

    IEnumerator WaitToSetMapHud()
    {
        yield return new WaitForFixedUpdate();
        SetInitiativeFeed();
        SetMapHudCharacter();
    }

    public void SetMapHudCharacter()
    {
        HUD_Map_nameText.text = SelectedUnit.characterSheet.UnitStat_Name;

        if (SelectedUnit is Unit_VehicleMaster)
        {
            Unit_VehicleMaster x = (Unit_VehicleMaster)SelectedUnit;
            HUD_Map_nameText.text = x.characterSheet.UnitStat_Name + "(" + x.CurrentPilot_Character.UnitStat_Name + ")";
        }

        HUD_Map_nameText.color = MapIconColors[(int)SelectedUnit.characterSheet.UnitStat_FactionTag];

        HUD_Map_hpText.text = "Hp : " + SelectedUnit.characterSheet.UnitStat_HitPoints;
        HUD_Map_heldWeaponText.text = SelectedUnit.equippedWeapon.Item_Name;
        HUD_Map_accText.text = "Acc: " + (int)SelectedUnit.Calculated_WeaponAccuracy + "%";
    }

    public void SetInitiativeFeed()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            InitiatveOrderFeedBoxes[initiativeOrder.IndexOf(x)].text = x.characterSheet.UnitStat_Name;

            if (x is Unit_VehicleMaster)
            {
                Unit_VehicleMaster y = (Unit_VehicleMaster)x;
                if (y.CurrentPilot_Character != null)
                InitiatveOrderFeedBoxes[initiativeOrder.IndexOf(x)].text = y.characterSheet.UnitStat_Name + "(" + y.CurrentPilot_Character.UnitStat_Name + ")";
            }

            InitiatveOrderFeedBoxes[initiativeOrder.IndexOf(x)].color = MapIconColors[(int)x.characterSheet.UnitStat_FactionTag];
        }
    }

    public void DeactivateSuppressors()
    {
        //foreach (Unit_Master x in initiativeOrder)
        //{
        //    x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_Waiting;
        //}
    }

    public void DeactivateMapCam()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isInMapMode = false;

        AudioListener tempCamAudioListener = MapCamera.GetComponent<AudioListener>();
        tempCamAudioListener.enabled = false;

        PlayerHUD.gameObject.SetActive(true);

        MapHUD.SetActive(false);
        InitiativeOrderBar.SetActive(false);
        MapCamera.targetTexture = (RenderTexture)MiniMap.texture;

        HUD_Player_ConfirmText.SetActive(false);

        ActivateNewUnit();
    }
    #endregion

    #region Phase 3 - Unit Acts
    //Activates the units who are suppress firing the controlled unit
    public void ActivateSuppressors()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            if (x.Current_Unit_Suppression_State != Unit_Master.Unit_Suppression_States.State_Waiting)
            {
                if (x.suppressionTarget == SelectedUnit)
                    x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_Suppressing;
                else
                    x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_WaitingToSuppress;
            }
        }
    }

    //Ends the unit's turn and calls FindNextActionsToActivate as a result
    public void EndUnitTurn()
    {
        SelectedUnit.Current_Unit_State = Unit_Master.Unit_States.State_Waiting;
        SelectedUnit.CameraAudioListener.enabled = false;

        TurnIsEnding = true;
        MiniMapTransformReset();
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
            if (SelectedUnit != null)
            SelectedUnit.ToggleControl(false);

            if (initiativeOrder[SelectedUnitIndex].isDead == false)
            {
                SelectedUnit = initiativeOrder[SelectedUnitIndex];

                for (int i = 0; i < initiativeOrder.Count; i++)
                {
                    if (initiativeOrder[i] != SelectedUnit)
                    {
                        if (initiativeOrder[i] != null)
                        initiativeOrder[i].ToggleControl(false);
                    }
                }

                ActivateMapCam();
            }

            else
            {
                SelectNewUnitToActivate();
            }
        }
    }

    void ActivateNewUnit()
    {
        TurnIsEnding = false;
        SelectedUnit.ToggleControl(true);
        ActivateSuppressors();
    }
    #endregion

    #region Phase 4 - End of Round
    //Any clean up, then call RoundProcess
    void EndRound()
    {
        ResetUnitStateMachines();
        StartRound();
    }
    #endregion

    public void ResetUnitStateMachines()
    {
        foreach (Unit_Master x in initiativeOrder)
        {
            x.StopAllCoroutines();
            x.isOnSuppressionCooldown = false;
            x.shooting.StopAllCoroutines();
            x.shooting.isFiring = false;
            x.Current_Unit_State = Unit_Master.Unit_States.State_Waiting;
            x.Current_Unit_Suppression_State = Unit_Master.Unit_Suppression_States.State_Waiting;
        }
    }

    public void ToggleExitApplicationCanvas()
    {
        if (ExitApplicationPanel.activeSelf)
        {
            ExitApplicationPanel.SetActive(false);
        }

        else
            ExitApplicationPanel.SetActive(true);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void CheckWinCondition()
    {
        int PlayerUnits = 0;
        int EnemyUnits = 0;

        foreach (Unit_Master x in initiativeOrder)
        {
            if (x.isDead == false)
            {
                if (x.characterSheet.UnitStat_FactionTag == Character_Master.FactionTag.SER)
                {
                    PlayerUnits++;
                }

                if (x.characterSheet.UnitStat_FactionTag != Character_Master.FactionTag.SER)
                {
                    EnemyUnits++;
                }
            }
        }

        if (PlayerUnits == 0 || EnemyUnits == 0)
        {
            EndBattle();
        }
    }

    public void EndBattle()
    {
        EndOfBattleCanvas.SetActive(true);
    }
}