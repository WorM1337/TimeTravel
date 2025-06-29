using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Collider2D))]
public class SceneChangeTrigger : MonoBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Player>() != null)
        {
            SceneChanger.instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
