using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Platform : MonoBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void AllowCollision(Collider2D otherCollider)
    {
        Physics2D.IgnoreCollision(_collider, otherCollider, false);
    }
    public void ForbidCollision(Collider2D otherCollider)
    {
        Physics2D.IgnoreCollision(_collider, otherCollider, true);  
    }
}
