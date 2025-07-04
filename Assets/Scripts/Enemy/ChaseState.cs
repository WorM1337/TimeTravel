using UnityEngine;

public class ChaseState : IEnemyState
{
    private Enemy _enemy;

    public void Enter(Enemy enemy)
    {
        _enemy = enemy;
        _enemy.animator.SetBool("Move", true);
    }

    public void Update()
    {

        if (_enemy.IsChaseableInAttackRadius)
        {
            _enemy.SwitchState(new AttackState());
            return;
        }
        else if (!_enemy.IsChaseableInSearchRadius)
        {
            _enemy.SwitchState(new IdleState());
            return;
        }

        float direction = (_enemy.CurrentChaseObj.transform.position.x - _enemy.transform.position.x);

        if (direction < 0 && _enemy.facingRight || direction > 0 && !_enemy.facingRight)
        {
            _enemy.Flip();
        }

        _enemy.MoveTowards(Mathf.Sign(direction));

        
    }

    public void Exit()
    {
        _enemy.animator.SetBool("Move", false);
        _enemy.StopMoving();
    }
}
