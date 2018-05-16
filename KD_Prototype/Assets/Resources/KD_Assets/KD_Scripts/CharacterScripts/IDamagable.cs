using UnityEngine;

public interface IDamagable
{
    void TakeDamage(int Damage, Item_Master.DamageTypes DamageType, string Attacker);
}
