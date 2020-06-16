using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint_Unit_Noah : Blueprint_Unit_Master
{
    Blueprint_Unit_Noah()
    {
        unitPrefabType = KD_Global.UnitPrefabType.Unit_Human_Prefab;
        infantrySheet = KD_Global.Characters.Character_SCRAPS_Noah;
    }
}
