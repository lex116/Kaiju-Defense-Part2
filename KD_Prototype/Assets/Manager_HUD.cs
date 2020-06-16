using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_HUD : MonoBehaviour
{
    public Camera mapCamera;
    public List<Unit_Master> hud_InitiativeOrder = new List<Unit_Master>();
    //public Unit_Master hud_SelectedUnit;
    public Overlord_Master hud_SelectedOverlord;

    #region Map Hud Fields
    [Header("Map Hud Fields")]

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

    public GameObject PlayerTargetHUD;

    internal float HUD_Player_Target_DisplayHp;
    internal float HUD_Player_Target_TargetHp;
    internal float HUD_Player_Target_DisplayNerve;
    internal float HUD_Player_Target_TargetNerve;
    #endregion

    public bool Inventory_isDispayed;
    public GameObject Inventory_Panel;
    #region Inventory Fields
    public Text Inventory_Character_Name;
    public Text Inventory_Character_Faction;
    public Text Inventory_Character_HP;
    public Text Inventory_Character_Nerve;
    public Text Inventory_Character_Reaction;
    public Text Inventory_Character_Accuracy;
    public Text Inventory_Character_Willpower;
    public Text Inventory_Character_Fitness;

    public Text Inventory_Weapon_Name;
    public Text Inventory_Weapon_Damage;
    public Text Inventory_Weapon_Range;

    public Text Inventory_Equipment_Name;
    public Text Inventory_Equipment_Damage;
    public Text Inventory_Equipment_EffectRadius;

    public Text Inventory_Armor_Name;
    public Text Inventory_Armor_RadiationResist;
    public Text Inventory_Armor_ExplosiveResist;
    public Text Inventory_Armor_ShredResist;
    public Text Inventory_Armor_HeatResist;
    public Text Inventory_Armor_ElectricResist;
    public Text Inventory_Armor_BluntResist;
    public Text Inventory_Armor_LightResist;
    public Text Inventory_Armor_PunctureResist;
    #endregion

    public Text MoraleText;
    public GameObject EndOfBattleCanvas;

    public Image Reticle;

    public GameObject Player_HUD_Basic;
    public GameObject Player_HUD_Moving;
    public GameObject Player_HUD_Action;
    public GameObject Player_HUD_Shooting;
    public GameObject Player_HUD_Equipment;

    internal Color32 ActionIcon_Selected_Opacity = new Color32(255, 255, 255, 255);
    internal Color32 ActionIcon_NotSelected_Opacity = new Color32(255, 255, 255, 50);

    [SerializeField]
    List<Image> ActionTray = new List<Image>();


    public GameObject InitiativeOrderBar;

    public Text ProbableAmountOfDamageToDeal_Text;

    public GameObject ExitApplicationPanel;

    public Text RemainingAP;

    #region Notification Feed
    [Header("Notification Feed")]

    [SerializeField]
    public List<Text> KillFeedBoxes = new List<Text>();

    public RawImage MiniMap;

    public Button activateUnitButton;
    #endregion

    private void Awake()
    {
        //ExitApplicationPanel.SetActive(false);
    }

    public void HUD_StrategicModeToggle(bool toggle)
    {
        if (toggle)
        {
            ResetPlayerHUD();
            PlayerHUD.gameObject.SetActive(false);
            MapHUD.SetActive(true);
            InitiativeOrderBar.SetActive(true);
            ClearInitiativeFeed();
            CalculateMorale();
            StartCoroutine(WaitToSetMapHud());
        }

        if (!toggle)
        {
            PlayerHUD.gameObject.SetActive(true);
            MapHUD.SetActive(false);
            InitiativeOrderBar.SetActive(false);
            HUD_Player_ConfirmText.SetActive(false);
        }
    }

   //this gets used by gamestate manager setround
   public void HUD_SetRound()
    {
        SetMiniMapDefaults();
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

    public void SetMiniMapDefaults()
    {
        MiniMap_Default_posX = MiniMap.rectTransform.anchoredPosition.x;
        MiniMap_Default_posY = MiniMap.rectTransform.anchoredPosition.y;
        MiniMap_Default_Width = MiniMap.rectTransform.sizeDelta.x;
        MiniMap_Default_Height = MiniMap.rectTransform.sizeDelta.y;
        miniMapIsDefault = true;
    }

    public void PlayerHudUpdate()
    {
        HUD_Player_heldWeaponText.text = hud_SelectedOverlord.ccu.equippedWeapon.Item_Name;

        if (hud_SelectedOverlord.ccu is Unit_VehicleMaster)
        {
            Unit_VehicleMaster x = (Unit_VehicleMaster)hud_SelectedOverlord.ccu;

            if (x.CurrentPilot_Character != null)
                HUD_Player_nameText.text = x.characterSheet.UnitStat_Name + "(" + x.CurrentPilot_Character.UnitStat_Name + ")";
        }

        HUD_Player_heldEquipmentText.text = hud_SelectedOverlord.ccu.equippedEquipment.Item_Name + ": " + hud_SelectedOverlord.ccu.equippedEquipment.Ammo;

        HUD_Player_nameText.text = hud_SelectedOverlord.ccu.characterSheet.UnitStat_Name;

        HUD_Player_ActionText.text = "" + hud_SelectedOverlord.ccu.Selected_Unit_Action.Action_Name;
        HUD_Player_StateText.text = "" + hud_SelectedOverlord.ccu.Current_Unit_State;

        PlayerStatUpdates();

        ToggleTargetHUD();

        RemainingAP.text = "AP: " + hud_SelectedOverlord.ccu.AP + " -> " + (hud_SelectedOverlord.ccu.AP - hud_SelectedOverlord.ccu.Selected_Unit_Action.Action_AP_Cost);
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

    public void ToggleExitApplicationCanvas()
    {
        if (ExitApplicationPanel.activeSelf)
        {
            ExitApplicationPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        else
        {
            ExitApplicationPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void MiniMapTransformReset()
    {
        miniMapIsDefault = true;
        MiniMap.rectTransform.anchoredPosition = new Vector3(MiniMap_Default_posX, MiniMap_Default_posY, 0);
        MiniMap.rectTransform.sizeDelta = new Vector2(MiniMap_Default_Width, MiniMap_Default_Height);
    }

    public void ResetNotificationFeed()
    {
        foreach (Text x in KillFeedBoxes)
        {
            x.text = "";
        }
    }

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


    //change this to not need a reference to the initiative order its potentially unneeded
    public void CalculateMorale()
    {
        float MaxMorale_SER = 0;
        float CurrentMorale_SER = 0;
        float MoralePercentage_SER = 0f;

        float MaxMorale_Enemy = 0;
        float CurrentMorale_Enemy = 0;
        float MoralePercentage_Enemy = 0f;

        foreach (var x in hud_InitiativeOrder)
        {
            if (x.isDead == false)
            {
                if (x.characterSheet.UnitStat_FactionTag == KD_Global.FactionTag.SER)
                {
                    MaxMorale_SER = x.characterSheet.UnitStat_StartingNerve + MaxMorale_SER;
                    CurrentMorale_SER = x.characterSheet.UnitStat_Nerve + CurrentMorale_SER;

                    MoralePercentage_SER = CurrentMorale_SER / MaxMorale_SER;
                }

                if (x.characterSheet.UnitStat_FactionTag != KD_Global.FactionTag.SER)
                {
                    MaxMorale_Enemy = x.characterSheet.UnitStat_StartingNerve + MaxMorale_Enemy;
                    CurrentMorale_Enemy = x.characterSheet.UnitStat_Nerve + CurrentMorale_Enemy;

                    MoralePercentage_Enemy = CurrentMorale_Enemy / MaxMorale_Enemy;
                }
            }
        }

        //Debug.Log("SER: " + MaxMorale_SER + " / " + CurrentMorale_SER);
        //Debug.Log("Enemy: " + MaxMorale_Enemy + " / " + CurrentMorale_Enemy);
        MoraleText.text = "SER:" + (int)(MoralePercentage_SER * 100)
            + " VS " + "Enemy:" + (int)(MoralePercentage_Enemy * 100);
    }

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
        HUD_Player_staminaBar.fillAmount = hud_SelectedOverlord.ccu.movementPointsRemaining / hud_SelectedOverlord.ccu.startingMovementPoints;

        HUD_Player_staminaText.text = ((int)hud_SelectedOverlord.ccu.movementPointsRemaining).ToString();
    }
    void UpdatePlayerNerve()
    {
        float currentNerve = 0;
        float startingNerve = 0;

        if (hud_SelectedOverlord.ccu.characterSheet.unitType == Character_Master.UnitTypes.Human)
        {
            currentNerve = hud_SelectedOverlord.ccu.characterSheet.UnitStat_Nerve;
            startingNerve = hud_SelectedOverlord.ccu.characterSheet.UnitStat_StartingNerve;

            HUD_Player_nerveText.text = hud_SelectedOverlord.ccu.characterSheet.UnitStat_Nerve.ToString();
        }

        if (hud_SelectedOverlord.ccu.characterSheet.unitType == Character_Master.UnitTypes.Vehicle ||
            hud_SelectedOverlord.ccu.characterSheet.unitType == Character_Master.UnitTypes.Mecha)
        {
            Unit_VehicleMaster SelectedVehicle = (Unit_VehicleMaster)hud_SelectedOverlord.ccu;
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
        float currentHp = hud_SelectedOverlord.ccu.characterSheet.UnitStat_HitPoints;
        float startingHp = hud_SelectedOverlord.ccu.characterSheet.UnitStat_StartingHitPoints;

        HUD_Player_TargetHp = (currentHp / startingHp);

        if (HUD_Player_DisplayHp > HUD_Player_TargetHp)
            HUD_Player_DisplayHp = HUD_Player_DisplayHp - HUD_Player_UpdateRate;

        if (HUD_Player_DisplayHp < HUD_Player_TargetHp)
            HUD_Player_DisplayHp = HUD_Player_DisplayHp + HUD_Player_UpdateRate;

        HUD_Player_healthBar.fillAmount = HUD_Player_DisplayHp;

        HUD_Player_hpText.text = hud_SelectedOverlord.ccu.characterSheet.UnitStat_HitPoints.ToString();
    }
    void UpdatePlayerAcc()
    {
        HUD_Player_TargetAcc = hud_SelectedOverlord.ccu.Calculated_WeaponAccuracy * hud_SelectedOverlord.ccu.CurrentShotAccuracyModifier;

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
            ActionTray[i].sprite = hud_SelectedOverlord.ccu.Unit_Actions[i + 1].Action_Icon;

            if (i + 1 == Array.IndexOf(hud_SelectedOverlord.ccu.Unit_Actions, hud_SelectedOverlord.ccu.Selected_Unit_Action))
                ActionTray[i].color = ActionIcon_Selected_Opacity;

            else
                ActionTray[i].color = ActionIcon_NotSelected_Opacity;
        }
    }


    public void ToggleTargetHUD()
    {
        if (hud_SelectedOverlord.LookedAtUnit_Master != null || hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint != null)
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

        if (hud_SelectedOverlord.LookedAtUnit_Master != null)
            DmgToDealWithResist = hud_SelectedOverlord.ccu.equippedWeapon.Damage - hud_SelectedOverlord.LookedAtUnit_Master.equippedArmor.DamageResistance[(int)hud_SelectedOverlord.ccu.equippedWeapon.damageType];

        if (hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint != null)
            DmgToDealWithResist = hud_SelectedOverlord.ccu.equippedWeapon.Damage - hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.AttachedArmor.DamageResistance[(int)hud_SelectedOverlord.ccu.equippedWeapon.damageType];

        int ProbableNumberOfShotsToHit = 
            (int)Math.Ceiling(((hud_SelectedOverlord.ccu.equippedWeapon.ShotCount * hud_SelectedOverlord.ccu.equippedWeapon.BurstCount) * ((hud_SelectedOverlord.ccu.Calculated_WeaponAccuracy * hud_SelectedOverlord.ccu.CurrentShotAccuracyModifier) / 100)));

        int DmgToDealTimesShotCount = DmgToDealWithResist * ProbableNumberOfShotsToHit;

        if (DmgToDealTimesShotCount < 0)
            DmgToDealTimesShotCount = 0;

        ProbableAmountOfDamageToDeal_Text.text = "Avg Dmg: " + (int)DmgToDealTimesShotCount + " (" + DmgToDealWithResist + " DMG x " + ProbableNumberOfShotsToHit + " Shots)";
    }
    void UpdatePlayerTargetName()
    {
        if (hud_SelectedOverlord.LookedAtUnit_Master != null)
            HUD_Player_Target_nameText.text = hud_SelectedOverlord.LookedAtUnit_Master.characterSheet.UnitStat_Name;

        if (hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint != null)
            HUD_Player_Target_nameText.text = hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.HardPointName;
    }
    void UpdatePlayerTargetHp()
    { 
        float currentHp = 0;
        float startingHp = 0;

        if (hud_SelectedOverlord.LookedAtUnit_Master != null)
        {
            currentHp = hud_SelectedOverlord.LookedAtUnit_Master.characterSheet.UnitStat_HitPoints;
            startingHp = hud_SelectedOverlord.LookedAtUnit_Master.characterSheet.UnitStat_StartingHitPoints;

            HUD_Player_Target_hpText.text = hud_SelectedOverlord.LookedAtUnit_Master.characterSheet.UnitStat_HitPoints.ToString();
        }

        if (hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint != null)
        {
            currentHp = hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.HitPoints;
            startingHp = hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.StartingHitPoints;

            HUD_Player_Target_hpText.text = hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.HitPoints.ToString();
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

        if (hud_SelectedOverlord.LookedAtUnit_Master != null)
        {
            currentNerve = hud_SelectedOverlord.LookedAtUnit_Master.characterSheet.UnitStat_Nerve;
            startingNerve = hud_SelectedOverlord.LookedAtUnit_Master.characterSheet.UnitStat_StartingNerve;

            HUD_Player_Target_nerveText.text = hud_SelectedOverlord.LookedAtUnit_Master.characterSheet.UnitStat_Nerve.ToString();
        }

        if (hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint != null)
        {

            if (hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character != null)
            {
                currentNerve = hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character.UnitStat_Nerve;
                startingNerve = hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character.UnitStat_StartingNerve;

                HUD_Player_Target_nerveText.text = hud_SelectedOverlord.LookedAtUnit_VehicleHardPoint.OwnerVehicle.CurrentPilot_Character.UnitStat_Nerve.ToString();
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

    IEnumerator WaitToSetMapHud()
    {
        yield return new WaitForFixedUpdate();
        SetInitiativeFeed();
        SetMapHudCharacter();
    }

    public void SetInitiativeFeed()
    {
        foreach (Unit_Master x in hud_InitiativeOrder)
        {
            InitiatveOrderFeedBoxes[hud_InitiativeOrder.IndexOf(x)].text = x.characterSheet.UnitStat_Name;

            if (x is Unit_VehicleMaster)
            {
                Unit_VehicleMaster y = (Unit_VehicleMaster)x;
                if (y.CurrentPilot_Character != null)
                    InitiatveOrderFeedBoxes[hud_InitiativeOrder.IndexOf(x)].text = y.characterSheet.UnitStat_Name + "(" + y.CurrentPilot_Character.UnitStat_Name + ")";
            }

            InitiatveOrderFeedBoxes[hud_InitiativeOrder.IndexOf(x)].color = KD_Global.MapIconColors[(int)x.characterSheet.UnitStat_FactionTag];
        }
    }
    public void SetMapHudCharacter()
    {
        HUD_Map_nameText.text = hud_SelectedOverlord.ccu.characterSheet.UnitStat_Name;

        if (hud_SelectedOverlord.ccu is Unit_VehicleMaster)
        {
            Unit_VehicleMaster x = (Unit_VehicleMaster)hud_SelectedOverlord.ccu;
            HUD_Map_nameText.text = x.characterSheet.UnitStat_Name + "(" + x.CurrentPilot_Character.UnitStat_Name + ")";
        }

        HUD_Map_nameText.color = KD_Global.MapIconColors[(int)hud_SelectedOverlord.ccu.characterSheet.UnitStat_FactionTag];

        HUD_Map_hpText.text = "Hp : " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_HitPoints;
        HUD_Map_heldWeaponText.text = hud_SelectedOverlord.ccu.equippedWeapon.Item_Name;
        HUD_Map_accText.text = "Acc: " + (int)hud_SelectedOverlord.ccu.Calculated_WeaponAccuracy + "%";
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

    public void ToggleInventoryScreen()
    {
        if (Inventory_isDispayed == false)
        {
            FillInventoryScreen();
            Inventory_isDispayed = true;
            Inventory_Panel.SetActive(true);
        }

        else if (Inventory_isDispayed == true)
        {
            Inventory_isDispayed = false;
            Inventory_Panel.SetActive(false);
        }
    }

    public void FillInventoryScreen()
    {
        Inventory_Character_Name.text = "Name: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_Name;
        Inventory_Character_Faction.text = "Faction: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_FactionTag.ToString();
        Inventory_Character_HP.text = "HP: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_HitPoints.ToString();
        Inventory_Character_Nerve.text = "Nerve: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_Nerve.ToString();
        Inventory_Character_Reaction.text = "Reaction: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_Reaction.ToString();
        Inventory_Character_Accuracy.text = "Accuracy: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_Accuracy.ToString();
        Inventory_Character_Willpower.text = "Willpower: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_Willpower.ToString();
        Inventory_Character_Fitness.text = "Fitness: " + hud_SelectedOverlord.ccu.characterSheet.UnitStat_Fitness.ToString();

        Inventory_Weapon_Name.text = hud_SelectedOverlord.ccu.equippedWeapon.Item_Name;
        Inventory_Weapon_Damage.text =
            (hud_SelectedOverlord.ccu.equippedWeapon.Damage * hud_SelectedOverlord.ccu.equippedWeapon.BurstCount * hud_SelectedOverlord.ccu.equippedWeapon.ShotCount)
            + " " + hud_SelectedOverlord.ccu.equippedWeapon.damageType.ToString() + " Dmg";
        Inventory_Weapon_Range.text = "Range: " + hud_SelectedOverlord.ccu.equippedWeapon.Range.ToString();

        Inventory_Equipment_Name.text = hud_SelectedOverlord.ccu.equippedEquipment.Item_Name;
        Inventory_Equipment_Damage.text =
            hud_SelectedOverlord.ccu.equippedEquipment.Damage + " " + hud_SelectedOverlord.ccu.equippedEquipment.damageType.ToString() + " Dmg";
        if (hud_SelectedOverlord.ccu.equippedEquipment.Damage == 0)
            Inventory_Equipment_Damage.text = "-- Dmg";
        Inventory_Equipment_EffectRadius.text = "Effect Range: " + hud_SelectedOverlord.ccu.equippedEquipment.EffectRadius.ToString();

        Inventory_Armor_Name.text = hud_SelectedOverlord.ccu.equippedArmor.Item_Name;
        Inventory_Armor_RadiationResist.text = "RAD: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[0].ToString();
        Inventory_Armor_ExplosiveResist.text = "EXP: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[1].ToString();
        Inventory_Armor_ShredResist.text = "SHRD: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[2].ToString();
        Inventory_Armor_HeatResist.text = "HEAT: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[3].ToString();
        Inventory_Armor_ElectricResist.text = "ELEC: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[4].ToString();
        Inventory_Armor_BluntResist.text = "BLNT: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[5].ToString();
        Inventory_Armor_LightResist.text = "LGHT: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[6].ToString();
        Inventory_Armor_PunctureResist.text = "PNCT: " + hud_SelectedOverlord.ccu.equippedArmor.DamageResistance[7].ToString();
    }

}
