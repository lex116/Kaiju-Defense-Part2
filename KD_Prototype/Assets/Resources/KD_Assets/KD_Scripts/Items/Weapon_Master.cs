﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Master : Item_Master
{
    public int Accuracy;
    public int ShotCount;
    public int BurstCount;
    public float FireRate;
    public enum FireModes
    {
        SingleShot,
        SpreadShot,
        AoeShot
    }
    public FireModes fireMode;

    public Sprite Reticle_Sprite;

    public GameObject WeaponModel;
}
