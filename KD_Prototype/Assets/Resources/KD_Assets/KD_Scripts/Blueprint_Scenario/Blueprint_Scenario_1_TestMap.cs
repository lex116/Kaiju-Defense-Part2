using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint_Scenario_1_TestMap : Blueprint_Scenario_Master
{
    Blueprint_Scenario_1_TestMap()
    {
        scenarioName = "1 Test Map";
        scenarioMap_Name = "Example_ScenarioMap";
        playerOverlordFaction = KD_Global.FactionTag.SER;
        aiOverlordFaction = KD_Global.FactionTag.SCRAPS;

        playerBlueprintUnits.Add(KD_Global.Blueprint_Units.Blueprint_Unit_Szymon);
        //playerBlueprintUnits.Add(KD_Global.Blueprint_Units.Blueprint_Unit_Szymon);
        enemyBlueprintUnits.Add(KD_Global.Blueprint_Units.Blueprint_Unit_Noah);
        //enemyBlueprintUnits.Add(KD_Global.Blueprint_Units.Blueprint_Unit_Samuel);

        //playerUnits.Add("Szymon");
        //playerUnits.Add("Szymon");
        //enemyUnits.Add("Szymon");
        //enemyUnits.Add("Szymon");
        //playerUnits;
        //enemyUnits = null;
}
}
