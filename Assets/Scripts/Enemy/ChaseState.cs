using UnityEngine;

public class ChaseState : IEnemyState
{
    private Enemy _enemy;

    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        Debug.Log("Враг начал преследование!");
    }

    public void Update()
    {
        Enemy enemy = _enemy;

        float direction = (enemy.player.position.x - enemy.transform.position.x);
        enemy.MoveTowards(Mathf.Sign(direction));

        if (enemy.IsPlayerInAttackRadius)
        {
            enemy.SwitchState(new AttackState());
        }
        else if (!enemy.IsPlayerInSearchRadius)
        {
            enemy.SwitchState(new PatrolState());
        }
    }

    public void Exit()
    {
        _enemy.StopMoving();
    }
}
