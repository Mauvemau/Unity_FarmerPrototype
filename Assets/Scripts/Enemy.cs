using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject targetRef;
    [SerializeField] private float speed = 1.0f;

    private Rigidbody2D _rb;

    public void SetTarget(GameObject targetRef)
    {
        this.targetRef = targetRef;
    }

    private void Awake()
    {
        if(TryGetComponent<Rigidbody2D>(out var rb))
        {
            _rb = rb;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!targetRef) return;
        if(col.gameObject.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage();
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!_rb) return;
        Vector2 direction = (targetRef.transform.position - transform.position).normalized;
        Vector2 newPosition = _rb.position + direction * speed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);
    }
}
