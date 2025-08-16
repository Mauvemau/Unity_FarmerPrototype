using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Color startColor;
    [SerializeField] private Color targetColor;
    [SerializeField] private AnimationCurve crossFadeCurve;
    [SerializeField, Range(0f, 10f)] private float crossFadeDuration;

    private MeshRenderer meshRenderer;
    private Coroutine crossFadeRoutine;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = startColor;
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (crossFadeRoutine != null)
                StopCrossFade();

            crossFadeRoutine = StartCoroutine(DoCrossFading());
        }
    }


    private IEnumerator DoCrossFading ()
    {
        float timer = 0f;

        while (timer < crossFadeDuration)
        {
            float t = crossFadeCurve.Evaluate(timer / crossFadeDuration);

            meshRenderer.material.color = Color.Lerp(startColor, targetColor, t);

            yield return new WaitForEndOfFrame();
            
            timer += Time.deltaTime;
        }

        FinishCrossFade();
    }

    private void StopCrossFade ()
    {
        StopCoroutine(crossFadeRoutine);
        FinishCrossFade();
    }

    private void FinishCrossFade ()
    {
        meshRenderer.material.color = targetColor;
        (targetColor, startColor) = (startColor, targetColor);
        crossFadeRoutine = null;
    }
}
