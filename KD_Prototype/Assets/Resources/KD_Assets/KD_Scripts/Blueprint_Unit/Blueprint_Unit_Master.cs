using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint_Unit_Master : ScriptableObject
{
    //type of unit to instance human, wheeled vehicle, mech
    public KD_Global.UnitPrefabType unitPrefabType;
    //the character sheet to instance for the human/pilot
    public KD_Global.Characters infantrySheet;
    //the character sheet to instance for the vehicle if there is one
    public KD_Global.Characters vehicleSheet;
}
