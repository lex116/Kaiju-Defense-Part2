using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlord_AI : Overlord_Master
{

    [SerializeField]
    List<Unit_Master> enemies_InSight = new List<Unit_Master>();

    int horde_Morale = 0; 

    //KD_Global.factionTag overlord_FactionTag;

    public enum tactic_States
    {
        Aggressive,
        Defensive,
        Regrouping,
        Searching
    };

    [SerializeField]
    public tactic_States tactic_Current;
}
