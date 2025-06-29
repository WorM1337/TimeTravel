using UnityEngine;
[RequireComponent (typeof(Collider2D))]
public class CheckPoint : MonoBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() is Player player)
        {
            player.RecentCheckPoint = transform.position;
        }
    }
}
