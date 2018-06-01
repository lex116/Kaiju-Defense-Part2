using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Vehicle_HeavyPlating : Armor_Master
{
    Armor_Vehicle_HeavyPlating()
    {
        Weight = 15;

        DamageResistance[(int)DamageTypes.Radiation] = 2;
        DamageResistance[(int)DamageTypes.Explosive] = 3;
        DamageResistance[(int)DamageTypes.Shred] = 4;
        DamageResistance[(int)DamageTypes.Heat] = 2;
        DamageResistance[(int)DamageTypes.Electrical] = 2;
        DamageResistance[(int)DamageTypes.Blunt] = 4;
        DamageResistance[(int)DamageTypes.Light] = 2;
        DamageResistance[(int)DamageTypes.Puncture] = 4;
    }
}
