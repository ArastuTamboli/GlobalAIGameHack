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

    [Header("Camera Bounds")]

    public Vector2 minBounds = new Vector2(-50f, -50f);
    public Vector2 maxBounds = new Vector2(50f, 50f);

    [Header("Zoom Bounds")]

    public Vector3 minZoom = new Vector3(0f, 5f, -5f);
    public Vector3 maxZoom = new Vector3(0f, 30f, -30f);

    bool inputEnabled = true;   
    void Start()
    {
        
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        targetZoom = cameraTransform.localPosition;
    }

    void Update()
    {
        if (inputEnabled)
        {
            HandleMovementInput();
            HandleMouseInput();
        }
        ApplyCameraTransforms();
    }
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
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


      
    }
    private void ApplyCameraTransforms()
    {
        targetPosition = ClampPosition(targetPosition);
        targetZoom = ClampZoom(targetZoom);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetZoom, Time.deltaTime * moveTime);
    }
    Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
        return position;
    }

    Vector3 ClampZoom(Vector3 zoom)
    {
        zoom.x = Mathf.Clamp(zoom.x, Mathf.Min(minZoom.x, maxZoom.x), Mathf.Max(minZoom.x, maxZoom.x));
        zoom.y = Mathf.Clamp(zoom.y, Mathf.Min(minZoom.y, maxZoom.y), Mathf.Max(minZoom.y, maxZoom.y));
        zoom.z = Mathf.Clamp(zoom.z, Mathf.Min(minZoom.z, maxZoom.z), Mathf.Max(minZoom.z, maxZoom.z));
        return zoom;
    }

}
