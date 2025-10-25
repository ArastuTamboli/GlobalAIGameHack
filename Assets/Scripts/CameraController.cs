using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;  

    [Header("Camera Settings")]
    public float moveSpeed;
    public float moveTime;

    public float rotationAmount;

    public Vector3 zoomAmount;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetZoom;

    private Vector3 dragStartPos;
    private Vector3 dragCurrentPos;

    private Vector3 rotateStartPos;
    private Vector3 rotateCurrentPos;
    void Start()
    {
        
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        targetZoom = cameraTransform.localPosition;
    }

    void Update()
    {
        HandleMovementInput();
        HandleMouseInput();
    }

    public void MoveToPosition(Vector3 gridPosition)
    {
        targetPosition = gridPosition;
    }
    public void HandleMouseInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            targetZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;    
            if(plane.Raycast(ray, out entry))
            {
                dragStartPos = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPos = ray.GetPoint(entry);
                targetPosition = transform.position + (dragStartPos - dragCurrentPos);

            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            rotateStartPos = Input.mousePosition;
        }
        if(Input.GetMouseButton(1))
        {
            rotateCurrentPos = Input.mousePosition;
            Vector3 difference = rotateStartPos - rotateCurrentPos;
            rotateStartPos = rotateCurrentPos;
            targetRotation *= Quaternion.Euler(Vector3.up * (-difference.x/5f));
        }
    }
    public void HandleMovementInput()
    {
        if(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow))
        {
            targetPosition += Vector3.forward * moveSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            targetPosition += Vector3.right * -moveSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            targetPosition += Vector3.forward * -moveSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            targetPosition += Vector3.right * moveSpeed;
        }

        if(Input.GetKey(KeyCode.Q))
        {
            targetRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            targetRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if(Input.GetKey(KeyCode.R))
        {
            targetZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            targetZoom += zoomAmount;
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetZoom, Time.deltaTime * moveTime);
    }
}
