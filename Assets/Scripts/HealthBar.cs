using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Image fillImage;

    private int maxValue;

    public void SetMaxValue(int amount) {
        maxValue = amount;
        SetValue(amount);
    }

    public void SetValue(int amount) {
        if (!fillImage) return;
        fillImage.fillAmount = Mathf.Clamp01((float)amount / maxValue);
    }
}
