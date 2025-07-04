using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startX;
    public float parallaxFactor = 0.5f;
    public Transform cam;

    private float width;
    private SpriteRenderer spriteRenderer;

    private bool isBackgroundRepeating = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        width = spriteRenderer.bounds.size.x;

        startX = cam.position.x;
    }

    void Update()
    {
        float dist = (cam.position.x * parallaxFactor);
        transform.position = new Vector3(startX + dist, transform.position.y, transform.position.z);

        if (cam.position.x > transform.position.x + width * 0.5f)
        {
            startX += width;
        }
        else if (cam.position.x < transform.position.x - width * 0.5f)
        {
            startX -= width;
        }
    }
}