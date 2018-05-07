using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Master : ScriptableObject
{
    public string Item_Name;
    public int Damage;
    [SerializeField]
    public enum DamageType
    {
        Radiation,
        Explosive,
        Shred,
        Heat,
        Electrical,
        Blunt,
        Light,
        Puncture
    };
    public DamageType damageType;
    public int Ammo;
    public int AmmoType;
    public int Weight;
    public int Range;
    public GameObject Projectile;
}
