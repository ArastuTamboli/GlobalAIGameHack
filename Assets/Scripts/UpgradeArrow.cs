using UnityEngine;

public class UpgradeArrow : MonoBehaviour
{
    [Header("Animation")]
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 2f;
    public float rotationSpeed = 90f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = startPosition + Vector3.up * bounce;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
