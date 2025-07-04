using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider2D))]
public class EventTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onTriggerActive;
    [SerializeField] private UnityEvent _onTriggerPerformed;
    [SerializeField] private UnityEvent _onTriggerCanceled;

    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IDamageable>() is IDamageable obj)
        {
            _onTriggerPerformed?.Invoke();
            
        }



    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() is IDamageable obj)
            _onTriggerActive?.Invoke();
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamageable>() is IDamageable obj)
            _onTriggerCanceled?.Invoke();
        
    }
}
