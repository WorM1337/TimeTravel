using UnityEngine;
using UnityEngine.Events;

public class ButtonE : MonoBehaviour, IRewindable, IInteractable
{
    [SerializeField] private UnityEvent[] _onButtonPerformedArray;
    private int _maxCountPress = 1; // 1 когда хотим, чтобы просто происходило событие по нажатию кнопки 
    private Animator _anim;

    private int counterPress = 0;

    private void Awake()
    {
        _maxCountPress = _onButtonPerformedArray.Length;
        _anim = GetComponent<Animator>();
    }
    private void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player player)
        {
            player.CurrentInteractable = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player player)
        {
            player.CurrentInteractable = null;
        }
    }

    public void CountPress()
    {
        counterPress++;
        while (counterPress >= _maxCountPress)
        {
           counterPress--;
        }
    }

    public void Press()
    {
        _onButtonPerformedArray[counterPress]?.Invoke();
        CountPress();
    }

    public void SaveState()
    {
        counterPress = counterPress;
    }

    public object GetState()
    {
        return new ButtonERewindState
        {
            counterPressed = counterPress,
        };
    }

    public void LoadState(object state)
    {
        var savedState = (ButtonERewindState)state;
        counterPress = savedState.counterPressed;
    }

    public void OnStartRewind()
    {

    }

    public void OnStopRewind()
    {

    }
}
public class ButtonERewindState
{
    public int counterPressed;
}
