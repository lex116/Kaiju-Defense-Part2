using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Szymon : Character_Master
{
    //Rookie Special Character
    Character_SER_Szymon()
    {
        UnitStat_FactionTag = KD_Global.FactionTag.SER;
        UnitStat_Name = "Szymon";
        UnitStat_StartingHitPoints = 24;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 97;
        UnitStat_Accuracy = 71;
        UnitStat_Willpower = 60;
        UnitStat_Fitness = 64;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 63;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_Pistol;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_Uniform;

        unitType = UnitTypes.Human;
    }
}
