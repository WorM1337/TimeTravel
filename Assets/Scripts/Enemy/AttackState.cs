using UnityEngine;

public class AttackState : IEnemyState
{
    private float timer;

    private Enemy _enemy;
    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        timer = _enemy.attackFirstDelay;
        _enemy.animator.SetBool("Attack", true);
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
            timer = _enemy.attackCulldown;
        }
    }

    public void Exit()
    {
        _enemy.animator.SetBool("Attack", false);
        // Можно добавить эффект окончания атаки
    }
}
