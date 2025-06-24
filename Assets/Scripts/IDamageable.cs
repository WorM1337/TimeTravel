using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
    void Heal(float healAmount);
    float GetMaxHealth();
}
