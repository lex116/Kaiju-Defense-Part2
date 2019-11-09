using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Wanderlei : Character_Master
{
    //SCRAP pilot
    Character_SCRAPS_Wanderlei()
    {
        UnitStat_FactionTag = KD_Global.FactionTag.SCRAPS;
        UnitStat_Name = "Wanderlei";
        UnitStat_StartingHitPoints = 21;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 51;
        UnitStat_Accuracy = 63;
        UnitStat_Willpower = 63;
        UnitStat_Fitness = 91;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 94;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_Pistol;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_Uniform;

        unitType = UnitTypes.Human;
    }   
}
