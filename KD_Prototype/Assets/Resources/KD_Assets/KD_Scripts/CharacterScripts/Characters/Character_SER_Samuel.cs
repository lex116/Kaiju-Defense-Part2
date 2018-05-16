﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Samuel : Character_Master
{
    //Vetern Special Character
    Character_SER_Samuel()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Samuel";
        UnitStat_StartingHitPoints = 19;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 62;
        UnitStat_Accuracy = 72;
        UnitStat_Willpower = 83;
        UnitStat_Fitness = 55;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 80;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_MachineGun;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_BodyArmor;

        unitType = UnitTypes.Human;
    }
}