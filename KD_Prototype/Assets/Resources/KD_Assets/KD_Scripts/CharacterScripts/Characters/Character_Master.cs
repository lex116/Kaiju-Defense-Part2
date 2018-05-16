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
        GSR
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
    //internal float startingMovementPoints;

    internal int UnitStat_Initiative;

    internal enum Weapons
    {
        Weapon_Human_Pistol,
        Weapon_Human_Shotgun,
        Weapon_Human_MachineGun,
        Weapon_Vehicle_MachineGun,
        Weapon_Human_AntiArmorRifle
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
}
