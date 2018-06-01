using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Mecha_Flyboy : Character_Master
{
    Character_SCRAPS_Mecha_Flyboy()
    {
        UnitStat_FactionTag = FactionTag.SCRAPS;
        UnitStat_Name = "Flyboy";
        UnitStat_StartingHitPoints = 180;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 75;
        UnitStat_Accuracy = 60;
        UnitStat_Willpower = 0;
        UnitStat_Fitness = 75;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 0;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Vehicle_MachineGun;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Vehicle_MediumPlating;

        unitType = UnitTypes.Mecha;

        Sensor_Armor = Armors.Armor_Vehicle_LightPlating;
        //Sensor_StartingHitPoints = 17;
        Sensor_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .35f);

        PrimaryWeapon_Armor = Armors.Armor_Vehicle_LightPlating;
        //PrimaryWeapon_StartingHitPoints = 42;
        PrimaryWeapon_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .45f);

        SecondaryEquipment_Armor = Armors.Armor_Vehicle_LightPlating;
        //SecondaryEquipment_StartingHitPoints = 42;
        SecondaryEquipment_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .45f);

        Locomotion_Armor = Armors.Armor_Vehicle_LightPlating;
        //Locomotion_StartingHitPoints = 68;
        Locomotion_StartingHitPoints = (int)(UnitStat_StartingHitPoints * .60f);
    }
}
