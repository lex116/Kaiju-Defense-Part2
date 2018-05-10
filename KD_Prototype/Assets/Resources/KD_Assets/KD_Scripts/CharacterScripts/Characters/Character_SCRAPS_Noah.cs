using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Noah : Character_Master
{
    //SCRAP soldier
    Character_SCRAPS_Noah()
    {
        UnitStat_FactionTag = FactionTag.SCRAPS;
        UnitStat_Name = "Noah";
        UnitStat_StartingHitPoints = 14;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 82;
        UnitStat_Accuracy = 50;
        UnitStat_Willpower = 71;
        UnitStat_Fitness = 30;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 83;
        //startingMovementPoints;

        selectedWeapon = DemoWeapon.Human_MachineGun;
        unitType = UnitTypes.Human;
    }
}
