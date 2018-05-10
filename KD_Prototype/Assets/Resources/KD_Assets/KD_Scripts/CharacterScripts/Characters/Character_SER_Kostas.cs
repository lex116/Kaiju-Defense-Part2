using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Kostas : Character_Master
{
    //Ser Pilot
    Character_SER_Kostas()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Kostas";
        UnitStat_StartingHitPoints = 17;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 72;
        UnitStat_Accuracy = 60;
        UnitStat_Willpower = 40;
        UnitStat_Fitness = 91;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 80;
        //startingMovementPoints;

        selectedWeapon = DemoWeapon.Human_Pistol;
        unitType = UnitTypes.Vehicle;
    }
}
