using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startX;
    public float parallaxFactor = 0.5f;
    public Transform cam;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        float dist = (cam.position.x * parallaxFactor);
        transform.position = new Vector3(startX + dist, transform.position.y, transform.position.z);

    }
}
