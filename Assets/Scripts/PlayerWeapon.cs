using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerWeapon : MonoBehaviour {
    [Header("Settings")] 
    [SerializeField] private Camera playerCam;
    
    [Header("Melee Attack Settings")] 
    [SerializeField, Min(0.1f)] private float meleeRange = 3f;
    [SerializeField, Range(10f, 360f)] private float meleeAttackAngle = 60f;

    [Header("Visual Settings")] 
    [SerializeField, Min(12)] private int arcSegments = 40;
    [SerializeField] private Color arcColor = Color.red;

    private Vector2 _aimDirection;
    private LineRenderer _lr;
    private MeshFilter _mf;
    
    private void DrawAttackArc() {
        if (!_mf) return;
        float aimAngle = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;
        float startAngle = aimAngle - meleeAttackAngle / 2f;

        Vector3[] vertices = new Vector3[arcSegments + 2];
        int[] triangles = new int[arcSegments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= arcSegments; i++) {
            float currentAngle = startAngle + (meleeAttackAngle / arcSegments) * i;
            float rad = currentAngle * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * meleeRange;
        }

        for (int i = 0; i < arcSegments; i++) {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        _mf.mesh = mesh;
    }
    
    private void DrawRangeCircle() {
        if (!_lr) return;
        int segments = arcSegments;
        _lr.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++) {
            float angle = (360f / segments) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * meleeRange;
            _lr.SetPosition(i, point);
        }
    }
    
    private void Update() {
        if (!playerCam) return;
        Vector3 mouseWorld = playerCam.ScreenToWorldPoint(Input.mousePosition);
        
        _aimDirection = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
        
        DrawRangeCircle();
        DrawAttackArc();
    }

    private void Awake() {
        if (!TryGetComponent<LineRenderer>(out var lr)) return;
        if (!TryGetComponent<MeshFilter>(out var mf)) return;
        if (!TryGetComponent<MeshRenderer>(out var mr)) return;
        _lr = lr;
        _lr.positionCount = 0;
        _lr.startWidth = 0.05f;
        _lr.endWidth = 0.05f;
        _lr.useWorldSpace = false;
        _lr.material = new Material(Shader.Find("Sprites/Default"));
        _lr.startColor = arcColor;
        _lr.endColor = arcColor;

        _mf = mf;
        mr.material = new Material(Shader.Find("Sprites/Default")) { color = arcColor };
    }
}
