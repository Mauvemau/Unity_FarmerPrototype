using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private HealthBar healthBarRef;
    [SerializeField] private GameObject targetRef;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private int maxHitPoints = 3;
    [SerializeField] private int currentHitPoints = 0;

    private Vector2 _pushVelocity;
    private Rigidbody2D _rb;
    
    public void Push(Vector2 direction, float force) {
        if (!_rb) return;
        _pushVelocity = direction * force;
    }

    public void TakeDamage(int hitPoints) {
        currentHitPoints -= hitPoints;
        CheckIsDead();
        if (currentHitPoints >= maxHitPoints) return;
        if (!healthBarRef) return;
        healthBarRef.gameObject.SetActive(true);
        healthBarRef.SetValue(currentHitPoints);
    }
    
    public void SetTarget(GameObject targetRef) {
        this.targetRef = targetRef;
    }

    private void CheckIsDead() {
        if (currentHitPoints > 0) return;
        Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D col) {
        if (!targetRef) return;
        if(col.gameObject.TryGetComponent<Player>(out var player)) {
            player.TakeDamage();
            Destroy(this.gameObject);
        }
    }
    
    private void FixedUpdate() {
        if (!_rb || !targetRef) return;

        Vector2 direction = (targetRef.transform.position - transform.position).normalized;
        Vector2 move = direction * (speed * Time.fixedDeltaTime);
        
        Vector2 newPosition = _rb.position + move + _pushVelocity * Time.fixedDeltaTime;

        _rb.MovePosition(newPosition);
        
        _pushVelocity *= 0.9f;
    }
    
    private void Awake() {
        if(TryGetComponent<Rigidbody2D>(out var rb)) {
            _rb = rb;
        }
        currentHitPoints = maxHitPoints;
        if (healthBarRef) {
            healthBarRef.SetMaxValue(maxHitPoints);
            healthBarRef.gameObject.SetActive(false);
        }
        else {
            Debug.LogWarning("Forgot to attach the health bar reference whoops");
        }
    }
}
