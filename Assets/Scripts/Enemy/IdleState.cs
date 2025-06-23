using UnityEngine;

public class IdleState : IEnemyState
{
    private Enemy _enemy;

    private float idleCounter;
    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        idleCounter = _enemy.idleTime;
    }

    public void Exit()
    {
        
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
