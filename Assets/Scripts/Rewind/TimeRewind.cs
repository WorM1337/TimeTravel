using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeRewind : MonoBehaviour
{
    private TimeRewindManager manager;
    private bool isRewinding = false;
    private bool isAbleToRewind = true;
    private InputSystem_Actions inputActions;
    private float rewindCooldown = 10f;
    private float cooldownTimer = 0f;
    private float rewindTimer = 0f;
    [SerializeField] private AudioSource rewindAudioSource;
    [SerializeField] private RewindUI rewindUI;

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

        if (rewindAudioSource == null)
        {
            rewindAudioSource = GetComponent<AudioSource>();
            if (rewindAudioSource == null)
            {
                Debug.LogWarning("AudioSource not found on TimeRewind object. Adding one automatically.");
                rewindAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        //cooldownTimer = 0f;
        rewindUI.ChangeRewindIconColor(Color.green);
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
        if (isRewinding)
        {
            rewindTimer += Time.unscaledDeltaTime;
            if (rewindTimer >= manager.RewindDuration)
            {
                StopRewind();
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
        if ((!isAbleToRewind) || isRewinding) return;

        rewindUI.ChangeRewindIconColor(Color.yellow);
        
        isRewinding = true;
        rewindTimer = 0f;
        manager.StartRewind();

        if (rewindAudioSource != null && rewindAudioSource.clip != null)
        {
            rewindAudioSource.Play();
        }
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
            //cooldownTimer = rewindCooldown;
            StartCoroutine(CooldownProcess());


            if (rewindAudioSource != null)
            {
                rewindAudioSource.Stop();
            }
        }
    }

    private IEnumerator CooldownProcess()
    {
        isAbleToRewind = false;
        rewindUI.ChangeRewindIconColor(Color.red);

        var counter = 0f;
        while(counter < rewindCooldown)
        {
            rewindUI.ChangeAngleOfFilling(1f - counter / rewindCooldown);
            counter += Time.unscaledDeltaTime;
            yield return null;
        }

        isAbleToRewind = true;
        rewindUI.ChangeRewindIconColor(Color.green);
    }
}