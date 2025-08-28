using UnityEngine;

public class HoverTransformEffect : MonoBehaviour {
    [Tooltip("Amplitude of the hover (how high it goes up and down)")]
    public float amplitude = 0.5f;
    [Tooltip("Speed of the hovering motion (cycles per second)")]
    public float frequency = 1f;
    [Tooltip("Defines how the motion progresses (linear = sharp, smooth = sine-like)")]
    public AnimationCurve motionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 originalLocalPosition;

    private void Awake() {
        originalLocalPosition = transform.localPosition;
    }

    private void Update() {
        float cycleTime = (Time.time * frequency) % 1f;
        float curvedTime = motionCurve.Evaluate(cycleTime);
        float yOffset = Mathf.Sin(curvedTime * Mathf.PI * 2f) * amplitude;
        
        transform.localPosition = originalLocalPosition + new Vector3(0, yOffset, 0);
    }
}
