using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Vehicle_HeavyPlating : Armor_Master
{
    Armor_Vehicle_HeavyPlating()
    {
        Weight = 10;

        DamageResistance[(int)DamageTypes.Radiation] = 1;
        DamageResistance[(int)DamageTypes.Explosive] = 2;
        DamageResistance[(int)DamageTypes.Shred] = 2;
        DamageResistance[(int)DamageTypes.Heat] = 1;
        DamageResistance[(int)DamageTypes.Electrical] = 0;
        DamageResistance[(int)DamageTypes.Blunt] = 3;
        DamageResistance[(int)DamageTypes.Light] = 0;
        DamageResistance[(int)DamageTypes.Puncture] = 3;
    }
}
