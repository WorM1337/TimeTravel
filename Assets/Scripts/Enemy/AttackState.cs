using UnityEngine;

public class AttackState : IEnemyState
{
    private float attackCooldown = 2f;
    private float timer;

    private Enemy _enemy;
    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        timer = 0;
        enemy.StopMoving();
        Debug.Log("Враг атакует!");
    }

    public void Update()
    {
        Enemy enemy = _enemy;

        timer -= Time.deltaTime;

        if (!enemy.IsPlayerInAttackRadius)
        {
            enemy.SwitchState(new ChaseState());
        }
        else if (timer <= 0)
        {
            Debug.Log("Нанесён урон игроку!");
            _enemy.player.gameObject.GetComponent<Player>().TakeDamage(_enemy.attackDamage);
            timer = attackCooldown;
        }
    }

    public void Exit()
    {
        // Можно добавить эффект окончания атаки
    }
}
