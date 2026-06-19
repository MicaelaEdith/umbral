using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    public UnityEvent OnClick;

    [SerializeField] private float hoverDarkenAmount = 0.85f;

    private SpriteRenderer sr;
    private Color defaultColor;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            defaultColor = sr.color;
    }

    private void OnMouseEnter()
    {
        if (sr == null) return;
        Color c = defaultColor;
        c.r *= hoverDarkenAmount;
        c.g *= hoverDarkenAmount;
        c.b *= hoverDarkenAmount;
        sr.color = c;
    }

    private void OnMouseExit()
    {
        if (sr == null) return;
        sr.color = defaultColor;
    }

    private void OnMouseDown()
    {
        OnClick?.Invoke();
    }
}
