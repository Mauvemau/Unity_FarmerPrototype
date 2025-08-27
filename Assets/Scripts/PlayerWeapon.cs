using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerWeapon : MonoBehaviour {
    [Header("Settings")] 
    [SerializeField] private Camera playerCam;
    
    [Header("Melee Attack Settings")] 
    [SerializeField, Min(0.1f)] private float meleeAttackRange = .75f;
    [SerializeField, Range(10f, 360f)] private float meleeAttackAngle = 60f;
    [SerializeField, Min(0)] private float attackRate = .75f; 
    [SerializeField, Min(0)] private float meleeAttackPushForce = 12f; 
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visual Settings")] 
    [SerializeField, Min(12)] private int arcSegments = 40;
    [SerializeField] private Color arcColor = Color.magenta;
    [SerializeField] private Color arcColorAttack = Color.white;
    [SerializeField] private AnimationCurve attackColorCrossFadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField, Min(0)] private float attackColorCrossFadeDuration = .3f;

    private Vector2 _aimDirection;
    private float nextAttack = 0;
    private LineRenderer _lr;
    private MeshFilter _mf;
    private MeshRenderer _mr;
    private Coroutine _colorCrossFadeRoutine;

    [SerializeField, HideInInspector]
    private CircleCollider2D colliderAttackRadius;

    private IEnumerator DoArcColorCrossFade() {
        _lr.startColor = arcColorAttack;
        _lr.endColor = arcColorAttack;
        _mr.material.color = arcColorAttack;

        float timer = 0f;

        while (timer < attackColorCrossFadeDuration) {
            float t = attackColorCrossFadeCurve.Evaluate(timer / attackColorCrossFadeDuration);
            
            Color current = Color.Lerp(arcColorAttack, arcColor, t);

            _lr.startColor = current;
            _lr.endColor = current;
            _mr.material.color = current;

            yield return null;
            timer += Time.deltaTime;
        }
        
        _lr.startColor = arcColor;
        _lr.endColor = arcColor;
        _mr.material.color = arcColor;

        _colorCrossFadeRoutine = null;
    }
    
    private void HandleCheatInput() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            meleeAttackAngle += 5f;
            if(meleeAttackAngle > 360f) meleeAttackAngle = 360f;
            Debug.Log($"Melee attack angle: {meleeAttackAngle} degrees");
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            meleeAttackAngle -= 5f;
            if(meleeAttackAngle < 10) meleeAttackAngle = 10f;
            Debug.Log($"Melee attack angle: {meleeAttackAngle} degrees");
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            meleeAttackRange += .1f;
            Debug.Log($"Melee attack range: {meleeAttackRange}");
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            meleeAttackRange -= .1f;
            if (meleeAttackRange < .15f) meleeAttackRange = .15f;
            Debug.Log($"Melee attack range: {meleeAttackRange}");
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            attackRate -= .05f;
            if (attackRate < .2f) attackRate = .2f;
            Debug.Log($"Melee attack rate: {attackRate}");
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            attackRate += .05f;
            Debug.Log($"Melee attack rate: {attackRate}");
        }
    }
    
    private void Attack() {
        if (Time.time < nextAttack) return;
        nextAttack = Time.time + attackRate;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            meleeAttackRange * transform.lossyScale.x,
            enemyLayer
        );

        bool fullCircle = Mathf.Approximately(meleeAttackAngle, 360f);

        foreach (var col in hits) {
            if (!col.TryGetComponent<Enemy>(out var enemy))
                continue;

            Vector2 toEnemy = ((Vector2)col.transform.position - (Vector2)transform.position);
            Vector2 pushDir = toEnemy.normalized;

            if (!fullCircle) {
                float angleToEnemy = Vector2.Angle(_aimDirection, pushDir);
                if (angleToEnemy > meleeAttackAngle / 2f)
                    continue;
            }
            
            enemy.TakeDamage(1);
            enemy.Push(pushDir, meleeAttackPushForce);
        }
        
        if (_colorCrossFadeRoutine != null)
            StopCoroutine(_colorCrossFadeRoutine);
        _colorCrossFadeRoutine = StartCoroutine(DoArcColorCrossFade());
    }
    
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

            vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * meleeAttackRange;
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
            Vector3 point = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * meleeAttackRange;
            _lr.SetPosition(i, point);
        }
    }
    
    private void Update() {
        HandleCheatInput();
        
        DrawRangeCircle();
        DrawAttackArc();
        
        Attack();
    }

    private void LateUpdate() {
        if (!playerCam) return;
        Vector3 mouseWorld = playerCam.ScreenToWorldPoint(Input.mousePosition);
        
        _aimDirection = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
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
        _mr = mr;
        _mr.material = new Material(Shader.Find("Sprites/Default")) { color = arcColor };
    }

    private void OnValidate() {
        if(!TryGetComponent<CircleCollider2D>(out var col)) return;
        colliderAttackRadius = col;
        colliderAttackRadius.isTrigger = true;
        colliderAttackRadius.radius = meleeAttackRange;
    }
}
