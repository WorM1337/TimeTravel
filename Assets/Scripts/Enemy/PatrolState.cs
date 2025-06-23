using UnityEngine;

public class PatrolState : IEnemyState
{
    private float patrolDirection = 1f;
    private float changeDirectionTime = 2f;
    private float timer;

    private Enemy _enemy;
    public void Enter(Enemy enemy)
    {
        _enemy = enemy;

        patrolDirection = 1f; // Начинаем с движения вправо

        changeDirectionTime = enemy.patrolTime;

        timer = changeDirectionTime;
    }

    public void Update()
    {
        Enemy enemy = _enemy;
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            patrolDirection *= -1;
            timer = changeDirectionTime;
        }

        enemy.MoveTowards(patrolDirection);

        if (enemy.IsPlayerInSearchRadius)
        {
            enemy.SwitchState(new ChaseState());
        }
    }

    public void Exit()
    {
        _enemy.StopMoving();
    }
}
