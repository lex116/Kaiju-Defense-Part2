using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Human_HeavyArmor : Armor_Master
{
    Armor_Human_HeavyArmor()
    {
        Item_Name = "Heavy Armor";
        Weight = 3;

        DamageResistance[(int)DamageTypes.Radiation] = 0;
        DamageResistance[(int)DamageTypes.Explosive] = 2;
        DamageResistance[(int)DamageTypes.Shred] = 2;
        DamageResistance[(int)DamageTypes.Heat] = 1;
        DamageResistance[(int)DamageTypes.Electrical] = 0;
        DamageResistance[(int)DamageTypes.Blunt] = 2;
        DamageResistance[(int)DamageTypes.Light] = 0;
        DamageResistance[(int)DamageTypes.Puncture] = 2;
    }

}
