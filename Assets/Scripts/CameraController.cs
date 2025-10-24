using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float moveSpeed = 10f;
    public float smoothTime = 0.3f;
    public float height = 15f;
    public float angle = 60f;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        transform.rotation = Quaternion.Euler(angle, 0, 0);
        targetPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void MoveToPosition(Vector3 gridPosition)
    {
        targetPosition = new Vector3(gridPosition.x, height, gridPosition.z - height / 2);
    }
}
