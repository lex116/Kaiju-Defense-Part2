using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Human_Uniform : Armor_Master
{ 
    public Armor_Human_Uniform()
    {
        Item_Name = "Uniform";
        Weight = 1;

        DamageResistance[(int)DamageTypes.Radiation] = 0;
        DamageResistance[(int)DamageTypes.Explosive] = 1;
        DamageResistance[(int)DamageTypes.Shred] = 0;
        DamageResistance[(int)DamageTypes.Heat] = 0;
        DamageResistance[(int)DamageTypes.Electrical] = 0;
        DamageResistance[(int)DamageTypes.Blunt] = 0;
        DamageResistance[(int)DamageTypes.Light] = 0;
        DamageResistance[(int)DamageTypes.Puncture] = 1;
    }
}
