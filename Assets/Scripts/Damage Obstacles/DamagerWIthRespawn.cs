using UnityEngine;

public class DamagerWIthRespawn : DamageObstacle
{
    [SerializeField] private float _damage;
    public override void DealDamage(IDamageable obj)
    {
        if(obj is Player)
        {
            obj.TakeDamage(_damage);
        }
        else
        {
            obj.TakeDamage(obj.GetMaxHealth());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<IDamageable>() is IDamageable obj)
        {
            DealDamage(obj);
            if(obj is Player player && player.GetCurrentHealth() != 0)
            {
                player.Respawn();
            }
        }
    }
}
