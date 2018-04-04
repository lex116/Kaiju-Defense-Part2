 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum DamageType { Piercing, Bludgening, Explosive };
public class Weapon : ScriptableObject
{
    public string Weapon_Name;
    public int Damage;
    public DamageType damageType;
    public float Accuracy;
    public int ShotCount;
    public int BurstCount;
    public int Range;
    public float FireRate;
    public enum FireModes
    {
        SingleShot,
        SpreadShot
    }
    public FireModes thisFireMode;

    //public string Weapon_Name { get; set; }
    //internal int Damage { get; set; }
    ////internal DamageType DamageType2 { get; set; }
    //internal float Accuracy { get; set; } //0-100
    //internal int ShotCount { get; set; }
    //internal int Range { get; set; }
    //internal float FireRate { get; set; }
}
