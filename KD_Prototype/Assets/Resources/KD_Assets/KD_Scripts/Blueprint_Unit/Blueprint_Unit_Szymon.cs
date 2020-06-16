using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint_Unit_Szymon : Blueprint_Unit_Master
{
    Blueprint_Unit_Szymon()
    {
        unitPrefabType = KD_Global.UnitPrefabType.Unit_Human_Prefab;
        infantrySheet = KD_Global.Characters.Character_SER_Szymon;
    }
}
