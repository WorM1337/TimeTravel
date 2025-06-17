using UnityEngine;

public class RewindableBlock : MonoBehaviour, IRewindable
{
    private Vector3 position;
    private Quaternion rotation;
    private Vector2 velocity;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning($"Rigidbody2D not found on {gameObject.name}. Rewind may not work correctly.");
        }
    }

    void Start()
    {
        if (TimeRewindManager.Instance != null)
        {
            TimeRewindManager.Instance.RegisterRewindable(this);
        }
        else
        {
            Debug.LogError($"TimeRewindManager.Instance is null! Ensure TimeRewindManager exists in the scene.");
        }
    }

    void OnDestroy()
    {
        if (TimeRewindManager.Instance != null)
        {
            TimeRewindManager.Instance.UnregisterRewindable(this);
        }
    }

    public void SaveState()
    {
        position = transform.position;
        rotation = transform.rotation;
        velocity = rb != null ? rb.linearVelocity : Vector2.zero;
    }

    public object GetState()
    {
        return new BlockState
        {
            position = position,
            rotation = rotation,
            velocity = velocity
        };
    }

    public void LoadState(object state)
    {
        var savedState = (BlockState)state;
        transform.position = savedState.position;
        transform.rotation = savedState.rotation;
        if (rb != null)
        {
            rb.linearVelocity = savedState.velocity;
        }
    }

    public void OnStartRewind()
    {
        if (rb != null)
        {
            rb.isKinematic = true; // Отключаем физику
        }
    }

    public void OnStopRewind()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Включаем физику
        }
    }
}

public class BlockState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;
}