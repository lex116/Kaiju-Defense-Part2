using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint_Scenario_Master : MonoBehaviour
{
    //name of the scenario
    string scenarioName;

    //class containing the spawns and their locations, also the actual map object to spawn
    //Map_Master scenarioMap;
    public GameObject scenarioMap;

    //Units to spawn at the start of the scenario
    public List<Blueprint_Unit_Master> scenarioUnits = new List<Blueprint_Unit_Master>();

    //player faction
    public Overlord_Master playerOverlord;
    public KD_Global.FactionTag playerOverlordFaction;

    //enemy faction
    public Overlord_Master enemyOverlord;
    public KD_Global.FactionTag enemyOverlordFaction;

    //number of enemy overlords to spawn
    int x;

    //set factions of overlords and spawn them
    void y() { }
}
