using UnityEngine;
using UnityEngine.InputSystem;

public class TimeRewind : MonoBehaviour
{
    private TimeRewindManager manager;
    private bool isRewinding = false;
    private InputSystem_Actions inputActions;
    [SerializeField] private float rewindCooldown = 5f; // Кулдаун на 5 секунд
    private float cooldownTimer = 0f;
    private float rewindTimer = 0f; // Таймер удержания клавиши

    void Awake()
    {
        manager = TimeRewindManager.Instance;
        if (manager == null)
        {
            Debug.LogError("TimeRewindManager.Instance is null! Ensure TimeRewindManager exists in the scene.");
            return;
        }
        inputActions = new InputSystem_Actions();
        inputActions.Player.Rewind.performed += ctx => StartRewind();
        inputActions.Player.Rewind.canceled += ctx => StopRewind();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isRewinding)
        {
            rewindTimer += Time.deltaTime;
            if (rewindTimer >= manager.RewindDuration)
            {
                StopRewind(); // Принудительно останавливаем, если превышено время
            }
        }
    }

    void FixedUpdate()
    {
        if (isRewinding && manager != null)
        {
            manager.Rewind();
        }
    }

    public void StartRewind()
    {
        if (manager == null)
        {
            Debug.LogError("Cannot start rewind: TimeRewindManager is null!");
            return;
        }
        if (cooldownTimer > 0f || isRewinding) return;
        isRewinding = true;
        rewindTimer = 0f;
        manager.StartRewind();
    }

    public void StopRewind()
    {
        if (manager == null)
        {
            Debug.LogError("Cannot stop rewind: TimeRewindManager is null!");
            return;
        }
        if (isRewinding)
        {
            isRewinding = false;
            rewindTimer = 0f;
            manager.StopRewind();
            cooldownTimer = rewindCooldown;
        }
    }
}