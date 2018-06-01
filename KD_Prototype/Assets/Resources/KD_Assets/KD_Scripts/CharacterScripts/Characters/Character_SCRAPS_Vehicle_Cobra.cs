using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Vehicle_Cobra : Character_Master
{
    Character_SCRAPS_Vehicle_Cobra()
    {
        UnitStat_FactionTag = FactionTag.SCRAPS;
        UnitStat_Name = "Cobra";
        UnitStat_StartingHitPoints = 140;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 75;
        UnitStat_Accuracy = 60;
        UnitStat_Willpower = 0;
        UnitStat_Fitness = 100;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 0;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Vehicle_Cannon;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Vehicle_HeavyPlating;

        unitType = UnitTypes.Vehicle;

        Sensor_Armor = Armors.Armor_Vehicle_HeavyPlating;
        //Sensor_StartingHitPoints = 14;
        Sensor_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .35f);

        PrimaryWeapon_Armor = Armors.Armor_Vehicle_HeavyPlating;
        //PrimaryWeapon_StartingHitPoints = 35;
        PrimaryWeapon_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .45f);

        SecondaryEquipment_Armor = Armors.Armor_Vehicle_HeavyPlating;
        //SecondaryEquipment_StartingHitPoints = 35;
        SecondaryEquipment_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .45f);

        Locomotion_Armor = Armors.Armor_Vehicle_HeavyPlating;
        //Locomotion_StartingHitPoints = 56;
        Locomotion_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .60f);
    }
}