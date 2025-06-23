using UnityEngine;

public class OnePunchDamager : DamageObstacle
{
    public override void DealDamage(Player player)
    {
        player.TakeDamage(player.GetMaxHealth());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Player>() is Player player)
        {
            DealDamage(player);
        }
    }
}
