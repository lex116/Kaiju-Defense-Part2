using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Mecha_Rogatina : Character_Master
{ 
    Character_SER_Mecha_Rogatina()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Rogatina";
        UnitStat_StartingHitPoints = 200;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 69;
        UnitStat_Accuracy = 90;
        UnitStat_Willpower = 0;
        UnitStat_Fitness = 75;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 0;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Vehicle_Cannon;
        selectedEquipment = Equipment.Equipment_Human_SmokeGrenadePack;
        selectedArmor = Armors.Armor_Vehicle_HeavyPlating;

        unitType = UnitTypes.Mecha;

        Sensor_Armor = Armors.Armor_Vehicle_MediumPlating;
        //Sensor_StartingHitPoints = 17;
        Sensor_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .35f);

        PrimaryWeapon_Armor = Armors.Armor_Vehicle_MediumPlating;
        //PrimaryWeapon_StartingHitPoints = 42;
        PrimaryWeapon_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .45f);

        SecondaryEquipment_Armor = Armors.Armor_Vehicle_MediumPlating;
        //SecondaryEquipment_StartingHitPoints = 42;
        SecondaryEquipment_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .45f);

        Locomotion_Armor = Armors.Armor_Vehicle_MediumPlating;
        //Locomotion_StartingHitPoints = 68;
        Locomotion_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .60f);
    }
}
