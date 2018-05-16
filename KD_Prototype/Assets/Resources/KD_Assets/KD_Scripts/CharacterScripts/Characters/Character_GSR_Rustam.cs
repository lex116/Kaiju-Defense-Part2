﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_GSR_Rustam : Character_Master
{
    //GSR Pilot 1
    Character_GSR_Rustam()
    {
        UnitStat_FactionTag = FactionTag.GSR;
        UnitStat_Name = "Rustam";
        UnitStat_StartingHitPoints = 25;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 80;
        UnitStat_Accuracy = 80;
        UnitStat_Willpower = 80;
        UnitStat_Fitness = 80;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 80;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_Pistol;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_HeavyArmor;

        unitType = UnitTypes.Human;
    }
}