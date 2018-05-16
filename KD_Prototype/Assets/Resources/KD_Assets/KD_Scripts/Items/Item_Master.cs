using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Master : ScriptableObject
{
    public string Item_Name;
    public int Damage;
    [SerializeField]
    public enum DamageTypes
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

    public DamageTypes damageType;
    public int Ammo;
    public int AmmoType;
    public int Weight;
    public int Range;
    internal GameObject Projectile;

    public void Awake()
    {
        SetUp();
    }

    public virtual void SetUp()
    {
        //
    }
}
