using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Kostas : Character_Master
{
    //Ser Pilot
    Character_SER_Kostas()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Kostas";
        UnitStat_StartingHitPoints = 23;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 72;
        UnitStat_Accuracy = 60;
        UnitStat_Willpower = 72;
        UnitStat_Fitness = 91;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 80;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_Pistol;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_Uniform;

        unitType = UnitTypes.Human;
    }
}
