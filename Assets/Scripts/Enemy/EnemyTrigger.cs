using UnityEngine;

[System.Serializable]
public enum TriggerType
{
    Outer,
    Inner
}

public class EnemyTrigger : MonoBehaviour
{
    public TriggerType Type = TriggerType.Outer;

    [SerializeField] private Enemy _enemy;

    private Collider2D _collider; // Если триггер внешний - коллайдер Circle, иначе коллайдер Capsule
                                  // И все они триггеры(isTrigger = true)!


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == _enemy.player.GetComponent<Collider2D>())
        {
            if (Type == TriggerType.Inner)
            {
                _enemy.IsPlayerInAttackRadius = true;
            }
            if (Type == TriggerType.Outer)
            {
                _enemy.IsPlayerInSearchRadius = true;
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == _enemy?.player?.GetComponent<Collider2D>())
        {
            if (Type == TriggerType.Inner)
            {
                _enemy.IsPlayerInAttackRadius = false;
            }
            if (Type == TriggerType.Outer)
            {
                _enemy.IsPlayerInSearchRadius = false;
            }
        }
    }
}
