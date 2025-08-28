using UnityEngine;

public class Player : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private SpriteRenderer playerSprite;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private AnimationCurve gripCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float slipDuration = 0.5f;
    
    private Rigidbody2D rb;
    private Vector2 inputDir;
    private Vector2 currentVelocity;
    private float slipTimer;
    
    public void TakeDamage() {
        Debug.Log("Player was hit");
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
    }

    private void HandleSpriteFlipping() {
        if (!playerSprite) return;
        if (inputDir.x == 0) return;
        playerSprite.flipX = inputDir.x switch {
            > 0 => false,
            < 0 => true,
            _ => playerSprite.flipX
        };
    }
    
    private void HandleInput() {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.W)) y += 1f;
        if (Input.GetKey(KeyCode.S)) y -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;

        inputDir = new Vector2(x, y).normalized;
    }

    private void HandlePhysics() {
        if (inputDir.sqrMagnitude > 0f) {
            slipTimer = 0f;
            currentVelocity = inputDir * moveSpeed;
        } 
        else {
            slipTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(slipTimer / slipDuration);
            float factor = 1f - gripCurve.Evaluate(t);
            currentVelocity *= factor;
        }


        rb.linearVelocity = currentVelocity;
    }

    private void Update() {
        HandleInput();
        HandleSpriteFlipping();
    }
    
    private void FixedUpdate() {
        HandlePhysics();
    }
}
