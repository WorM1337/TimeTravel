using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeRewind : MonoBehaviour
{
    [Header("Сколько секунд откатываем")]
    public float rewindDuration = 3f;

    [Header("Скорость обновления (раз в FixedUpdate)")]
    public float recordInterval = 0.02f;

    [Header("Кулдаун между откатами")]
    public float rewindCooldown = 5f;

    private float cooldownTimer = 0f;
    private float recordTimer = 0f;
    private bool isRewinding = false;

    private List<TransformSnapshot> snapshots = new List<TransformSnapshot>();
    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new InputSystem_Actions();
        inputActions.Player.Rewind.performed += _ => TryStartRewind();
        inputActions.Player.Rewind.canceled += _ => StopRewind();
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isRewinding)
            Rewind();
        else
            Record();
    }

    void Record()
    {
        recordTimer += Time.fixedDeltaTime;

        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;
            snapshots.Insert(0, new TransformSnapshot(transform.position, transform.rotation));
        }

        float maxSnapshots = rewindDuration / recordInterval;
        if (snapshots.Count > maxSnapshots)
            snapshots.RemoveAt(snapshots.Count - 1);
    }

    void Rewind()
    {
        if (snapshots.Count > 0)
        {
            TransformSnapshot snapshot = snapshots[0];
            transform.position = snapshot.position;
            transform.rotation = snapshot.rotation;
            snapshots.RemoveAt(0);
        }
        else
        {
            StopRewind();
        }
    }

    public void TryStartRewind()
    {
        if (cooldownTimer <= 0f && snapshots.Count > 0)
        {
            StartRewind();
        }
    }

    public void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
    }

    public void StopRewind()
    {
        if (isRewinding)
        {
            isRewinding = false;
            rb.isKinematic = false;
            cooldownTimer = rewindCooldown;
        }
    }

    private struct TransformSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformSnapshot(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
}
