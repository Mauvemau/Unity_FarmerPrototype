using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Movement Settings")] 
    [SerializeField] private GameObject targetReference;

    [Header("Collision Settings")]
    [SerializeField] private GameObject mapBoundsReference;

    private Bounds mapBounds;
    private Camera cam;

    private void Start() {
        cam = Camera.main;
        
        if (!mapBoundsReference) return;
        Renderer r = mapBoundsReference.GetComponent<Renderer>();
        if (r != null) {
            mapBounds = r.bounds;
        } else {
            Debug.LogError("Map bounds reference must have a Renderer component!");
        }
    }

    private void LateUpdate() {
        if (!targetReference || !cam) return;

        Vector3 targetPos = targetReference.transform.position;
        Vector3 newPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        
        if (mapBoundsReference) {
            float verticalExtent = cam.orthographicSize;
            float horizontalExtent = verticalExtent * cam.aspect;

            float minX = mapBounds.min.x + horizontalExtent;
            float maxX = mapBounds.max.x - horizontalExtent;
            float minY = mapBounds.min.y + verticalExtent;
            float maxY = mapBounds.max.y - verticalExtent;

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        }

        transform.position = newPos;
    }
}
