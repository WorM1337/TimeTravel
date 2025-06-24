using UnityEngine;

public class OnePunchDamager : DamageObstacle
{
    public override void DealDamage(IDamageable obj)
    {
        obj.TakeDamage(obj.GetMaxHealth());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<IDamageable>() is IDamageable obj)
        {
            DealDamage(obj);
        }
    }
}
