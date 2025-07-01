using UnityEngine;
using UnityEngine.Events;

public class ButtonE : MonoBehaviour
{
    [SerializeField] private UnityEvent _onButtonPerformed;

    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player player)
        {
            player.CurrentButtonE = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player player)
        {
            player.CurrentButtonE = null;
        }
    }

    public void Press()
    {
        _onButtonPerformed?.Invoke();
    }
}
