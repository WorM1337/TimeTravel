using UnityEngine;

public class ChaseState : IEnemyState
{
    private Enemy _enemy;

    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        _enemy.animator.SetBool("Move", true);
        Debug.Log("Враг начал преследование!");
    }

    public void Update()
    {
        Enemy enemy = _enemy;

        float direction = (enemy.player.position.x - enemy.transform.position.x);

        if (direction < 0 && _enemy.facingRight || direction > 0 && !_enemy.facingRight)
        {
            _enemy.Flip();
        }

        enemy.MoveTowards(Mathf.Sign(direction));

        if (enemy.IsPlayerInAttackRadius)
        {
            enemy.SwitchState(new AttackState());
        }
        else if (!enemy.IsPlayerInSearchRadius)
        {
            enemy.SwitchState(new IdleState());
        }
    }

    public void Exit()
    {
        _enemy.animator.SetBool("Move", false);
        _enemy.StopMoving();
    }
}
