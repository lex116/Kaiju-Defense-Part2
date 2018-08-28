using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Vehicle_LightPlating : Armor_Master
{
    Armor_Vehicle_LightPlating()
    {
        Item_Name = "Light Plating";
        Weight = 7;

        DamageResistance[(int)DamageTypes.Radiation] = 1;
        DamageResistance[(int)DamageTypes.Explosive] = 1;
        DamageResistance[(int)DamageTypes.Shred] = 1;
        DamageResistance[(int)DamageTypes.Heat] = 1;
        DamageResistance[(int)DamageTypes.Electrical] = 0;
        DamageResistance[(int)DamageTypes.Blunt] = 1;
        DamageResistance[(int)DamageTypes.Light] = 0;
        DamageResistance[(int)DamageTypes.Puncture] = 2;
    }
}
