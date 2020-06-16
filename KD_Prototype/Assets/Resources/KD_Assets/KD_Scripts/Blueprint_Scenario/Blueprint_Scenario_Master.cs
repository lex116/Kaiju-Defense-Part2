using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint_Scenario_Master : ScriptableObject
{
    //name of the scenario
    public string scenarioName;

    //class containing the spawns and their locations, also the actual map object to spawn
    public string scenarioMap_Name;

    //Units to spawn at the start of the scenario
    public List<KD_Global.Blueprint_Units> playerBlueprintUnits = new List<KD_Global.Blueprint_Units>();
    public List<KD_Global.Blueprint_Units> enemyBlueprintUnits = new List<KD_Global.Blueprint_Units>();

    //player faction
    public KD_Global.FactionTag playerOverlordFaction;
    
    //enemy faction
    public KD_Global.FactionTag aiOverlordFaction;
}
