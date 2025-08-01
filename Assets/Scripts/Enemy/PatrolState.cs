using UnityEngine;

public class PatrolState : IEnemyState
{
    private float patrolDirection;

    private Enemy _enemy;
    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        _enemy.animator.SetBool("Move", true);
        patrolDirection = (enemy.facingRight ? 1f : -1f); // �������� � �������� ������
    }

    public void Update()
    {

        _enemy.patrolCounter -= Time.deltaTime;

        if (_enemy.patrolCounter <= 0)
        {
            patrolDirection *= -1;
            _enemy.Flip();
            _enemy.patrolCounter = _enemy.patrolTime;
        }

        _enemy.MoveTowards(patrolDirection);

        if (_enemy.IsChaseableInSearchRadius)
        {
            _enemy.SwitchState(new ChaseState());
        }
    }

    public void Exit()
    {
        _enemy.animator.SetBool("Move", false);
        _enemy.StopMoving();
    }
}
