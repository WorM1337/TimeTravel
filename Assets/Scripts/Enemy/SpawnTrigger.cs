using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using UnityEngine;
[RequireComponent (typeof(Collider2D))]
public class SpawnTrigger : MonoBehaviour, IRewindable
{
    [SerializeField] private Enemy[] _groupOfEnemies;

    private bool enemiesIsActive;

    private bool enemiesWasDisabledAtFirst = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        
    }
    private void Start()
    {
        TimeRewindManager.Instance.RegisterRewindable(this);
    }
    private void Update()
    {
        if (!enemiesWasDisabledAtFirst)
        {
            enemiesIsActive = false;
            foreach (var enemy in _groupOfEnemies)
            {
                enemy.Die(); // Не умирает, а дизактевируется
            }
            TimeRewindManager.Instance.ClearStates();
            enemiesWasDisabledAtFirst = true;
        }
    }
    private void SwitchEnemies()
    {
        enemiesIsActive = !enemiesIsActive;
        Debug.Log($"{_groupOfEnemies.Length} - length array of group of enemies, {enemiesIsActive} - enemis activity");
        foreach (var enemy in _groupOfEnemies)
        {
            if (!enemiesIsActive)
                enemy.Die();
            else
                enemy.Respawn();
        }
    } 
    public object GetState()
    {
        return new SpawnTriggerRewindState
        {
            groupOfEnemiesIsActive = enemiesIsActive
        };
    }

    public void LoadState(object state)
    {
        var savedState = (SpawnTriggerRewindState)state;

        var oldEnemies = enemiesIsActive;

        if(oldEnemies != savedState.groupOfEnemiesIsActive)
        {
            SwitchEnemies();
            Debug.Log($"Rewind for trigger was exetuted: old {oldEnemies} new {savedState.groupOfEnemiesIsActive}");
        }
    }

    public void OnStartRewind()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    public void OnStopRewind()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public void SaveState()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {
            if(!enemiesIsActive) SwitchEnemies();
        }
    }
}
public class SpawnTriggerRewindState
{
    public bool groupOfEnemiesIsActive; 
}