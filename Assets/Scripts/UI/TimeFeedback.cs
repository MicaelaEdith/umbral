using UnityEngine;
using TMPro;
using System.Collections;

public class TimeFeedback : MonoBehaviour
{
    [SerializeField]
    private Color flashColor = Color.red;
    [SerializeField]
    private float flashDuration = 0.6f;
    [SerializeField]
    private float scaleAmount = 1.005f;

    private TextMeshProUGUI label;
    private Color originalColor;
    private Vector3 originalScale;
    private Coroutine currentFlash;

    private void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
        originalColor = label.color;
        originalScale = transform.localScale;
    }

    public void Flash()
    {
        if (currentFlash != null)
            StopCoroutine(currentFlash);

        currentFlash = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float half = flashDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            float t = elapsed / half;
            label.color = Color.Lerp(originalColor, flashColor, t);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleAmount, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        label.color = flashColor;
        transform.localScale = originalScale * scaleAmount;

        elapsed = 0f;
        while (elapsed < half)
        {
            float t = elapsed / half;
            label.color = Color.Lerp(flashColor, originalColor, t);
            transform.localScale = Vector3.Lerp(originalScale * scaleAmount, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        label.color = originalColor;
        transform.localScale = originalScale;
        currentFlash = null;
    }
}
