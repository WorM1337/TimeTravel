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
    }

    public void Update()
    {
        Enemy enemy = _enemy;

        timer -= Time.deltaTime;

        if (!enemy.IsChaseableInAttackRadius)
        {
            enemy.SwitchState(new ChaseState());
        }
        else if (timer <= 0)
        {
            if(_enemy.CurrentChaseObj == _enemy.player) _enemy.player.GetComponent<Player>().TakeDamage(_enemy.attackDamage);
            timer = _enemy.attackCulldown;
        }
    }

    public void Exit()
    {
        _enemy.animator.SetBool("Attack", false);
    }
}
