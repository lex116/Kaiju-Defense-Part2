using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Wanderlei : Character_Master
{
    //SCRAP pilot
    Character_SCRAPS_Wanderlei()
    {
        UnitStat_FactionTag = FactionTag.SCRAPS;
        UnitStat_Name = "Wanderlei";
        UnitStat_StartingHitPoints = 18;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 51;
        UnitStat_Accuracy = 40;
        UnitStat_Willpower = 11;
        UnitStat_Fitness = 91;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 94;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_MachineGun;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_HeavyArmor;

        unitType = UnitTypes.Human;
    }   
}
