using UnityEngine;

public class InspectManager : MonoBehaviour
{
    public Camera playerCamera;
    public Transform offsetPosition;
    public float inspectDistance = 3f;
    public LayerMask inspectLayer;
    public float zoomSpeed = 1f;
    public float minZoom = -2f;
    public float maxZoom = 5f;

    private Transform currentTarget;
    private bool isExamining = false;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private Vector3 lastMousePos;

    private Item currentItem;
    private float currentZoom = 0f;

    void Update()
    {
        if (!isExamining)
        {
            DetectInspectable();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isExamining)
            {
                StopInspecting();
            }
            else if (currentTarget != null)
            {
                StartInspecting();
            }
        }

        if (isExamining)
        {
            RotateInspectedObject();
            HandleZoom();
            CheckDistanceFromOriginal();
        }
    }

    void DetectInspectable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, inspectDistance, inspectLayer))
        {
            if (hit.collider.CompareTag("Item"))
            {
                currentTarget = hit.collider.transform;
                currentItem = currentTarget.GetComponent<Item>();
            }
        }
        else
        {
            currentTarget = null;
            currentItem = null;
        }
    }

    void StartInspecting()
    {
        isExamining = true;
        originalPos = currentTarget.position;
        originalRot = currentTarget.rotation;
        currentZoom = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentTarget.position = offsetPosition.position;

        lastMousePos = Input.mousePosition;

        Rigidbody rb = currentTarget.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // ENABLE THIS FOR NORMAL LEVELS
        if (currentItem != null)
        {
            
        }
        if(currentItem.itemName == "Flashlight" || currentItem.itemName == "Battery"){
            currentItem.MarkAsPickable();
        }
    }

    void StopInspecting()
    {
        isExamining = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentTarget != null)
        {
            currentTarget.position = originalPos;
            currentTarget.rotation = originalRot;

            Rigidbody rb = currentTarget.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }

        currentTarget = null;
        currentItem = null;
    }

    void RotateInspectedObject()
    {
        Vector3 delta = Input.mousePosition - lastMousePos;
        currentTarget.Rotate(Vector3.up, -delta.x * 0.5f, Space.World);
        currentTarget.Rotate(Vector3.right, delta.y * 0.5f, Space.World);
        lastMousePos = Input.mousePosition;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentZoom = Mathf.Clamp(currentZoom - scroll * zoomSpeed, minZoom, maxZoom);
            currentTarget.position = offsetPosition.position + playerCamera.transform.forward * currentZoom;
        }
    }

    void CheckDistanceFromOriginal()
    {
        float distance = Vector3.Distance(playerCamera.transform.position, originalPos);
        if (distance > inspectDistance + 0.5f)
        {
            StopInspecting();
        }
    }

    public void ForceStopInspecting()
    {
        if (!isExamining || currentTarget == null) return;
        StopInspecting();
    }
}
