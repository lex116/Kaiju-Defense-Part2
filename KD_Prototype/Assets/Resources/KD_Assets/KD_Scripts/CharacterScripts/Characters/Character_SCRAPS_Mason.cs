using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Mason : Character_Master
{
    //SCRAP soldier
    Character_SCRAPS_Mason()
    {
        UnitStat_FactionTag = KD_Global.FactionTag.SCRAPS;
        UnitStat_Name = "Mason";
        UnitStat_StartingHitPoints = 29;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 73;
        UnitStat_Accuracy = 72;
        UnitStat_Willpower = 75;
        UnitStat_Fitness = 90;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 94;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_Shotgun;
        selectedEquipment = Equipment.Equipment_Human_SmokeGrenadePack;
        selectedArmor = Armors.Armor_Human_HeavyArmor;

        unitType = UnitTypes.Human;
    }
}
