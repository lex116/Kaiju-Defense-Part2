using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Arlo : Character_Master
{
    //SCRAP pilot
    Character_SCRAPS_Arlo()
    {
        UnitStat_FactionTag = FactionTag.SCRAPS;
        UnitStat_Name = "Arlo";
        UnitStat_StartingHitPoints = 11;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 54;
        UnitStat_Accuracy = 57;
        UnitStat_Willpower = 72;
        UnitStat_Fitness = 32;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 87;
        //startingMovementPoints;

        selectedWeapon = DemoWeapon.Human_Pistol;
        unitType = UnitTypes.Vehicle;
    }
}
