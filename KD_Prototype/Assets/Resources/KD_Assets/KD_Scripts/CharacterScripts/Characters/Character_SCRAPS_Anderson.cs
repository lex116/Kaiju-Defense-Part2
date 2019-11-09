using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Anderson : Character_Master
{
    //SCRAP soldier
    Character_SCRAPS_Anderson()
    {
        UnitStat_FactionTag = KD_Global.FactionTag.SCRAPS;
        UnitStat_Name = "Anderson";
        UnitStat_StartingHitPoints = 26;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 79;
        UnitStat_Accuracy = 80;
        UnitStat_Willpower = 58;
        UnitStat_Fitness = 64;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 68;
        //startingMovementPoints;

        selectedWeapon = Weapons.Weapon_Human_MachineGun;
        selectedEquipment = Equipment.Equipment_Human_FragGrenadePack;
        selectedArmor = Armors.Armor_Human_BodyArmor;

        unitType = UnitTypes.Human;
    }
}
