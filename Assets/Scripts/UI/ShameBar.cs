using UnityEngine;
using UnityEngine.UI;

public class ShameBar : MonoBehaviour
{
    [SerializeField]
    private Image overlayImage;
    [SerializeField]
    private Color overlayColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField]
    [Range(0f, 1f)]
    private float maxOpacity = 0.27f;
    [SerializeField]
    private float pulseSpeed = 3f;
    [SerializeField]
    [Range(0f, 1f)]
    private float pulseMin = 0.75f;

    private void Update()
    {
        if (GameManager.Instance == null || overlayImage == null) return;

        bool timedShameActive = GameManager.Instance.shameTimerMinutes > 0
            || GameManager.Instance.IsShameFadingOut;

        float effectiveMaxOpacity = timedShameActive
            ? GameManager.Instance.timedShameOpacity
            : maxOpacity;

        if (timedShameActive)
        {
            float pulse = Mathf.Lerp(pulseMin, 1f, Mathf.PingPong(Time.time * pulseSpeed, 1f));
            effectiveMaxOpacity *= pulse;
        }

        float alpha = GameManager.Instance.shameActive
            ? Mathf.Clamp01(GameManager.Instance.shameLevel) * effectiveMaxOpacity
            : 0f;

        Color color = overlayColor;
        color.a = alpha;
        overlayImage.color = color;
    }
}
