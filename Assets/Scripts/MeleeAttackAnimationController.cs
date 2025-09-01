using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MeleeAttackAnimationController : MonoBehaviour {
    [Header("Settings")] 
    [SerializeField] private bool clockwise = false;
    [SerializeField] private float attackDuration = .25f;
    [SerializeField] private float spriteScaleOffset = 1f;
    [SerializeField] private AnimationCurve swingCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    private SpriteRenderer _spriteRenderer;
    private float _lastAttackTimestamp;
    private Coroutine _swingCoroutine;
    
    private IEnumerator SwingAnimation(float startAngle, float endAngle) {
        float elapsed = 0f;

        while (elapsed < attackDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / attackDuration);
            
            float curvedT = swingCurve.Evaluate(t);

            float currentAngle = Mathf.Lerp(startAngle, endAngle, curvedT);
            transform.eulerAngles = new Vector3(0, 0, currentAngle);
            yield return null;
        }

        _spriteRenderer.enabled = false;
    }
    
    public void PlayAttackAnimation(Vector2 attackDirection, float attackAngle, float attackRadius) {
        if (!_spriteRenderer) return;
        
        float centerDirectionAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        float startDirectionAngle = clockwise ? centerDirectionAngle + attackAngle / 2f : centerDirectionAngle - attackAngle / 2f;
        float endDirectionAngle = clockwise ? centerDirectionAngle - attackAngle / 2f : centerDirectionAngle + attackAngle / 2f;
        
        transform.localScale = new Vector3(attackRadius * spriteScaleOffset, attackRadius * spriteScaleOffset, 1);
        _spriteRenderer.enabled = true;
        if (_swingCoroutine != null) StopCoroutine(_swingCoroutine);
        _swingCoroutine = StartCoroutine(SwingAnimation(startDirectionAngle, endDirectionAngle));
    }
    
    private void Awake() { 
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
