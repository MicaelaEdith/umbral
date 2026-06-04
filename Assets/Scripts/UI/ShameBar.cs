using UnityEngine;
using UnityEngine.UI;

public class ShameBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float minWidth = 30f;
    [SerializeField] private float maxWidth = 600f;
    [SerializeField] private float barHeight = 20f;
    [SerializeField] private Color lowColor = new Color(0.53f, 0.81f, 0.92f);
    [SerializeField] private Color highColor = Color.red;

    private RectTransform fillRect;

    private void Awake()
    {
        if (fillImage != null)
        {
            fillRect = fillImage.GetComponent<RectTransform>();
            fillRect.pivot = new Vector2(0f, 0.5f);
            fillRect.anchorMin = new Vector2(0f, 0.5f);
            fillRect.anchorMax = new Vector2(0f, 0.5f);
            fillRect.anchoredPosition = Vector2.zero;
            fillRect.sizeDelta = new Vector2(0f, barHeight);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || fillRect == null) return;

        if (!GameManager.Instance.shameActive)
        {
            fillRect.sizeDelta = new Vector2(0f, barHeight);
            SetFillAlpha(0f);
            return;
        }

        float level = Mathf.Clamp01(GameManager.Instance.shameLevel);

        fillRect.sizeDelta = new Vector2(
            Mathf.Lerp(minWidth, maxWidth, level),
            barHeight
        );

        fillImage.color = Color.Lerp(lowColor, highColor, level);
        SetFillAlpha(1f);
    }

    private void SetFillAlpha(float alpha)
    {
        Color c = fillImage.color;
        c.a = alpha;
        fillImage.color = c;
    }
}
