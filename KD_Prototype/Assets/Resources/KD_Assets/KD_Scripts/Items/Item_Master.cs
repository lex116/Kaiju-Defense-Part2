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

    //temp
    [SerializeField]
    internal int Ammo = 100;
    public int AmmoType;
    public int Weight;
    public int Range;
    public int EffectRadius;
    internal GameObject Projectile;

    internal AudioClip Firing_Clip;
    internal AudioClip Reload_Clip;

    public void Awake()
    {
        SetUp();
    }

    public virtual void SetUp()
    {
        //
    }
}
