using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Mecha_Xiphos : Character_Master
{
    Character_SER_Mecha_Xiphos()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Xiphos";
        UnitStat_StartingHitPoints = 140;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 75;
        UnitStat_Accuracy = 54;
        UnitStat_Willpower = 0;
        UnitStat_Fitness = 68;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 0;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Vehicle_MachineGun;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Vehicle_LightPlating;

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
