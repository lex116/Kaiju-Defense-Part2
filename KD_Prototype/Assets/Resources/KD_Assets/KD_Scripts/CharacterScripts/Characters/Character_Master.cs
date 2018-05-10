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

    internal enum DemoWeapon
    {
        Human_Pistol,
        Human_Shotgun,
        Human_MachineGun,
        Vehicle_MachineGun,
        Human_AntiArmorRifle
    }
    internal DemoWeapon selectedWeapon;

    internal enum UnitTypes
    {
        Human,
        Vehicle,
        Mecha
    }
    internal UnitTypes unitType;
}
