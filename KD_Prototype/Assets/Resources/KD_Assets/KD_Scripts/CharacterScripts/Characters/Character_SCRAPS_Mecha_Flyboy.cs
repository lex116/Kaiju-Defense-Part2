using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Mecha_Flyboy : Character_Master
{
    Character_SCRAPS_Mecha_Flyboy()
    {
        UnitStat_FactionTag = FactionTag.SCRAPS;
        UnitStat_Name = "Flyboy";
        UnitStat_StartingHitPoints = 50;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 75;
        UnitStat_Accuracy = 60;
        UnitStat_Willpower = 0;
        UnitStat_Fitness = 70;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 0;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Vehicle_MachineGun;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Vehicle_LightPlating;

        unitType = UnitTypes.Mecha;
    }
}
