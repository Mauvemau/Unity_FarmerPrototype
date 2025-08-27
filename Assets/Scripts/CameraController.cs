using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Movement Settings")] 
    [SerializeField] private GameObject targetReference;
    [Header("Collision Settings")]
    [SerializeField] private GameObject mapBoundsReference;
    
    private void LateUpdate() {
        if (!targetReference) return;
        Vector3 newPos;
        newPos.x = targetReference.transform.position.x;
        newPos.y = targetReference.transform.position.y;
        newPos.z = transform.position.z;
        transform.position = newPos;

    }
}
