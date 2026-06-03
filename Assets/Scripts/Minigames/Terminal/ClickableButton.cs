using UnityEngine;

public class ClickableButton : MonoBehaviour
{
    [SerializeField] private float hoverAlpha = 0.7f;
    [SerializeField] private float pressedAlpha = 0.4f;
    [SerializeField] private bool isCancel;

    private SpriteRenderer spriteRenderer;
    private bool isPressed;
    private float normalAlpha;

    public bool IsPressed => isPressed;
    public bool IsInteractable { get; set; } = true;
    public bool IsCancel => isCancel;

    public event System.Action<ClickableButton> OnClick;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalAlpha = spriteRenderer != null ? spriteRenderer.color.a : 1f;
    }

    private void OnMouseEnter()
    {
        if (!IsInteractable || isPressed) return;
        SetAlpha(hoverAlpha);
    }

    private void OnMouseExit()
    {
        if (!IsInteractable || isPressed) return;
        SetAlpha(normalAlpha);
    }

    private void OnMouseDown()
    {
        if (!IsInteractable || isPressed) return;
        Press();
    }

    public void Press()
    {
        if (isPressed) return;
        isPressed = true;
        SetAlpha(pressedAlpha);
        OnClick?.Invoke(this);
    }

    public void PressNoNotify()
    {
        if (isPressed) return;
        isPressed = true;
        SetAlpha(pressedAlpha);
    }

    public void ResetButton()
    {
        isPressed = false;
        SetAlpha(normalAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer == null) return;
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
