using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_GSR_Fedor : Character_Master
{
    //GSR Pilot 1
    Character_GSR_Fedor()
    {
        UnitStat_FactionTag = FactionTag.GSR;
        UnitStat_Name = "Fedor";
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

        selectedWeapon = DemoWeapon.Human_Pistol;
        unitType = UnitTypes.Mecha;
    }
}
