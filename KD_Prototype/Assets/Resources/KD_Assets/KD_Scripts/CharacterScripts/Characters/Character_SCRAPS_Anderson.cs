using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SCRAPS_Anderson : Character_Master
{
    //SCRAP soldier
    Character_SCRAPS_Anderson()
    {
        UnitStat_FactionTag = FactionTag.SCRAPS;
        UnitStat_Name = "Anderson";
        UnitStat_StartingHitPoints = 19;
        //UnitStat_StartingActionPoints;
        //UnitStat_ActionPoints;
        UnitStat_Reaction = 44;
        UnitStat_Accuracy = 9;
        UnitStat_Willpower = 41;
        UnitStat_Fitness = 64;
        //UnitStat_Aptitude; ???
        //UnitStat_VisualRange;
        UnitStat_StartingNerve = 30;
        //startingMovementPoints;

        selectedWeapon = DemoWeapon.Human_MachineGun;
        unitType = UnitTypes.Human;
    }
}
