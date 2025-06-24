using UnityEngine;

public class HealthBarEnemy : MonoBehaviour
{
    [SerializeField] private GameObject _scalePart;

    public void SetHealthUI(float healthPart)
    {
        var vec = _scalePart.transform.localScale;
        _scalePart.transform.localScale = new Vector3(healthPart, vec.y, vec.z);
    }
    public void Flip()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, (transform.eulerAngles.y + 180f) % 360, 0f);
    }
}
