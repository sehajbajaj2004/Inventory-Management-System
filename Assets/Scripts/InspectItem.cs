using UnityEngine;

public class InspectManager : MonoBehaviour
{
    public Camera playerCamera;
    public Transform offsetPosition;
    public float inspectDistance = 3f;
    public LayerMask inspectLayer;

    private Transform currentTarget;
    private bool isExamining = false;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private Vector3 lastMousePos;

    private Item currentItem;

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

        if (currentItem != null)
        {
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
    }

    void RotateInspectedObject()
    {
        Vector3 delta = Input.mousePosition - lastMousePos;
        currentTarget.Rotate(Vector3.up, -delta.x * 0.5f, Space.World);
        currentTarget.Rotate(Vector3.right, delta.y * 0.5f, Space.World);
        lastMousePos = Input.mousePosition;
    }

    public void ForceStopInspecting()
    {
        if (!isExamining || currentTarget == null) return;

        StopInspecting();
        currentTarget = null;
        currentItem = null;
    }

}
