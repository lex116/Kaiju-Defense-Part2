using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Piercing, Bludgening, Explosive };

public abstract class Weapon {
    
    internal int Damage { get; set; }
    internal DamageType DamageType { get; set; }
    internal float Accuracy { get; set; } //0-100
    internal int ShotCount { get; set; }
}
