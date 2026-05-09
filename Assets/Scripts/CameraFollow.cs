using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Collider2D cameraBounds;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 nextPosition = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        transform.position = ClampToBounds(nextPosition);
    }

    Vector3 ClampToBounds(Vector3 desired)
    {
        if (cameraBounds == null || cam == null || !cam.orthographic) return desired;

        Bounds bounds = cameraBounds.bounds;
        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        float minX = bounds.min.x + cameraWidth;
        float maxX = bounds.max.x - cameraWidth;
        float minY = bounds.min.y + cameraHeight;
        float maxY = bounds.max.y - cameraHeight;

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
}
