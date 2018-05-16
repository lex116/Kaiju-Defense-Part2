using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Thomas : Character_Master
{
    //Vetern Special Character
    Character_SER_Thomas()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Thomas";
        UnitStat_StartingHitPoints = 17;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 88;
        UnitStat_Accuracy = 86;
        UnitStat_Willpower = 59;
        UnitStat_Fitness = 60;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 31;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_AntiArmorRifle;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_Uniform;

        unitType = UnitTypes.Human;
    }
}