using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Human_BodyArmor : Armor_Master
{
    Armor_Human_BodyArmor()
    {
        Weight = 2;

        DamageResistance[(int)DamageTypes.Radiation] = 0;
        DamageResistance[(int)DamageTypes.Explosive] = 2;
        DamageResistance[(int)DamageTypes.Shred] = 1;
        DamageResistance[(int)DamageTypes.Heat] = 0;
        DamageResistance[(int)DamageTypes.Electrical] = 0;
        DamageResistance[(int)DamageTypes.Blunt] = 1;
        DamageResistance[(int)DamageTypes.Light] = 0;
        DamageResistance[(int)DamageTypes.Puncture] = 2;
    }
}
