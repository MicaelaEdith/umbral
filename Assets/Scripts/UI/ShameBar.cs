using UnityEngine;
using UnityEngine.UI;

public class ShameBar : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private Color overlayColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField, Range(0f, 1f)] private float maxOpacity = 0.27f;

    private void Update()
    {
        if (GameManager.Instance == null || overlayImage == null) return;

        float alpha = GameManager.Instance.shameActive
            ? Mathf.Clamp01(GameManager.Instance.shameLevel) * maxOpacity
            : 0f;

        Color color = overlayColor;
        color.a = alpha;
        overlayImage.color = color;
    }
}
