using UnityEngine;

public class IdleState : IEnemyState
{
    private Enemy _enemy;

    private float idleCounter;
    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        _enemy.animator.SetBool("Idle", true);
        idleCounter = _enemy.idleTime;
    }

    public void Exit()
    {
        _enemy.animator.SetBool("Idle", false);
    }

    public void Update()
    {
        idleCounter -= Time.deltaTime;

        if (idleCounter < 0)
        {
            _enemy.SwitchState(new PatrolState());
        }

    }
}
