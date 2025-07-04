using Unity.VisualScripting;
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


    private LayerMask _visibleLayerMask;


    private void Awake()
    {
        _visibleLayerMask = LayerMask.GetMask("Default", "Ground");
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Chaseable>())
        {

            if (Type == TriggerType.Inner)
            {
                _enemy.IsChaseableInAttackRadius = true;
            }
            if (Type == TriggerType.Outer)
            {

                Vector2 direction = (collision.transform.position - _enemy.transform.position).normalized;

                RaycastHit2D hitForSearch = Physics2D.Raycast
                (
                    _enemy.gameObject.transform.position,
                    direction,
                    Vector2.Distance(collision.transform.position, _enemy.transform.position),
                    _visibleLayerMask
                );

                if(hitForSearch.collider == null)
                {
                    _enemy.IsChaseableInSearchRadius = true;
                    _enemy.CurrentChaseObj = collision.gameObject;
                }
                else
                {
                    Debug.Log($"Коллайдер на пути: {hitForSearch.collider.gameObject}");
                }
                
            }
        }
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Chaseable>())
        {
            if (Type == TriggerType.Outer)
            {
                var circle = GetComponent<CircleCollider2D>();

                Vector2 direction = (collision.transform.position - _enemy.transform.position).normalized;

                RaycastHit2D hitForSearch = Physics2D.Raycast
                (
                    _enemy.gameObject.transform.position,
                    direction,
                    Vector2.Distance(collision.transform.position, _enemy.transform.position),
                    _visibleLayerMask
                );
                if (hitForSearch.collider == null)
                {
                    _enemy.IsChaseableInSearchRadius = true;
                    _enemy.CurrentChaseObj = collision.gameObject;
                }
                else if(_enemy.CurrentChaseObj == collision.gameObject)
                {
                    _enemy.IsChaseableInSearchRadius = false;
                    _enemy.CurrentChaseObj = null;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_enemy.CurrentChaseObj == collision.gameObject)
        {
            if (Type == TriggerType.Inner)
            {
                _enemy.IsChaseableInAttackRadius = false;
            }
            if (Type == TriggerType.Outer)
            {
                _enemy.IsChaseableInSearchRadius = false;
                _enemy.CurrentChaseObj = null;
            }

        }
    }
}
