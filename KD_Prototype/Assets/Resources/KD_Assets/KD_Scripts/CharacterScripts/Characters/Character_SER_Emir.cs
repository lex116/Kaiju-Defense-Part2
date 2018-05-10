using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Emir : Character_Master
{
    //Soldier Character
    Character_SER_Emir()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Emir";
        UnitStat_StartingHitPoints = 21;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 81;
        UnitStat_Accuracy = 64;
        UnitStat_Willpower = 84;
        UnitStat_Fitness = 74;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 83;
        //startingMovementPoints;

        selectedWeapon = DemoWeapon.Human_MachineGun;
        unitType = UnitTypes.Human;
    }
}
