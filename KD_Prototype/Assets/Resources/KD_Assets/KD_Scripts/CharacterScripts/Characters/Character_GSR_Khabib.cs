using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_GSR_Khabib : Character_Master
{
    //GSR Pilot 2
    Character_GSR_Khabib()
    {
        UnitStat_FactionTag = KD_Global.FactionTag.GSR;
        UnitStat_Name = "Khabib";
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
