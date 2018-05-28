using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Master : ScriptableObject
{
    internal enum FactionTag
    {
        SER,
        SCRAPS,
        GSR,
        Neutral
    }
    internal FactionTag UnitStat_FactionTag;

    internal string UnitStat_Name;
    internal int UnitStat_StartingHitPoints;
    internal int UnitStat_HitPoints;
    //public int UnitStat_StartingActionPoints;
    //public int UnitStat_ActionPoints;
    internal int UnitStat_Reaction;
    internal int UnitStat_Accuracy;
    internal int UnitStat_Willpower;
    internal int UnitStat_Fitness;
    //public int UnitStat_Aptitude; ???
    //internal int UnitStat_VisualRange;
    internal int UnitStat_StartingNerve;
    internal int UnitStat_Nerve;
    internal bool isPanicked; 

    [SerializeField]
    internal int UnitStat_Initiative;

    internal enum Weapons
    {
        Weapon_Human_Pistol,
        Weapon_Human_Shotgun,
        Weapon_Human_MachineGun,
        Weapon_Vehicle_MachineGun,
        Weapon_Human_AntiArmorRifle,
        Weapon_Vehicle_Cannon
    }
    internal Weapons selectedWeapon;

    internal enum Equipment
    {
        Equipment_Human_FragGrenadePack
    }
    internal Equipment selectedEquipment;

    internal enum Armors
    {
        Armor_Human_Uniform,
        Armor_Human_BodyArmor,
        Armor_Human_HeavyArmor,
        Armor_Vehicle_LightPlating,
        Armor_Vehicle_MediumPlating,
        Armor_Vehicle_HeavyPlating
    }
    internal Armors selectedArmor;

    internal enum UnitTypes
    {
        Human,
        Vehicle,
        Mecha
    }
    internal UnitTypes unitType;

    //Vehicle Fields
    internal Armors Sensor_Armor;
    internal int Sensor_StartingHitPoints;
    internal Armors PrimaryWeapon_Armor;
    internal int PrimaryWeapon_StartingHitPoints;
    internal Armors SecondaryEquipment_Armor;
    internal int SecondaryEquipment_StartingHitPoints;
    internal Armors Locomotion_Armor;
    internal int Locomotion_StartingHitPoints;

    //initiative
    internal int initiativeRoll;
    internal bool initiativeRolled;
}
