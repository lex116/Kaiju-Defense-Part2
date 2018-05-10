using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SER_Szymon : Character_Master
{
    //Rookie Special Character
    Character_SER_Szymon()
    {
        UnitStat_FactionTag = FactionTag.SER;
        UnitStat_Name = "Szymon";
        UnitStat_StartingHitPoints = 21;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 97;
        UnitStat_Accuracy = 71;
        UnitStat_Willpower = 60;
        UnitStat_Fitness = 50;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 63;
        //startingMovementPoints;

        selectedWeapon = DemoWeapon.Human_Pistol;
        unitType = UnitTypes.Mecha;
    }
}
