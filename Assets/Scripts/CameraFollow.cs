using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Collider2D cameraBounds;
    public bool detachFromParentOnStart = false;
    public string fallbackBoundsObjectName = "background";
    public float leftBoundsPadding = 0f;
    public float rightBoundsPadding = 0f;
    public float bottomBoundsPadding = 0f;
    public float topBoundsPadding = 0f;

    private Camera cam;
    private Collider2D resolvedCameraBounds;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (detachFromParentOnStart)
            transform.SetParent(null, true);
    }

    void Start()
    {
        ResolveCameraBounds();
        transform.position = ClampToBounds(transform.position);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 nextPosition = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        transform.position = ClampToBounds(nextPosition);
    }

    void OnPreCull()
    {
        transform.position = ClampToBounds(transform.position);
    }

    Vector3 ClampToBounds(Vector3 desired)
    {
        Collider2D boundsCollider = GetCameraBounds();
        if (boundsCollider == null || cam == null || !cam.orthographic) return desired;

        Bounds bounds = boundsCollider.bounds;
        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        float minX = bounds.min.x + cameraWidth + leftBoundsPadding;
        float maxX = bounds.max.x - cameraWidth - rightBoundsPadding;
        float minY = bounds.min.y + cameraHeight + bottomBoundsPadding;
        float maxY = bounds.max.y - cameraHeight - topBoundsPadding;

        if (minX > maxX)
            desired.x = bounds.center.x;
        else
            desired.x = Mathf.Clamp(desired.x, minX, maxX);

        if (minY > maxY)
            desired.y = bounds.center.y;
        else
            desired.y = Mathf.Clamp(desired.y, minY, maxY);

        return desired;
    }

    Collider2D GetCameraBounds()
    {
        if (cameraBounds != null)
            return cameraBounds;

        if (resolvedCameraBounds == null)
            ResolveCameraBounds();

        return resolvedCameraBounds;
    }

    void ResolveCameraBounds()
    {
        if (cameraBounds != null)
        {
            resolvedCameraBounds = cameraBounds;
            return;
        }

        Collider2D[] colliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);

        foreach (Collider2D collider in colliders)
        {
            if (IsFallbackBoundsObject(collider.transform))
            {
                resolvedCameraBounds = collider;
                return;
            }
        }
    }

    bool IsFallbackBoundsObject(Transform current)
    {
        while (current != null)
        {
            if (current.name == fallbackBoundsObjectName)
                return true;

            current = current.parent;
        }

        return false;
    }
}
