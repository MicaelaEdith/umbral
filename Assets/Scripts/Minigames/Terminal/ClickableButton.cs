using UnityEngine;

public class ClickableButton : MonoBehaviour
{
    [SerializeField]
    private float hoverAlpha = 0.7f;
    [SerializeField]
    private float pressedAlpha = 0.4f;
    [SerializeField]
    private bool isCancel;
    [SerializeField]
    private Sprite pressedSprite;

    private SpriteRenderer spriteRenderer;
    private bool isPressed;
    private float normalAlpha;
    private Sprite originalSprite;

    public bool IsPressed => isPressed;
    public bool IsInteractable { get; set; } = true;
    public bool IsCancel => isCancel;

    public event System.Action<ClickableButton> OnClick;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            normalAlpha = spriteRenderer.color.a;
            originalSprite = spriteRenderer.sprite;
        }
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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (!IsInteractable || isPressed) return;
        Press();
    }

    public void Press()
    {
        if (isPressed) return;
        isPressed = true;
        SetPressedVisuals();
        OnClick?.Invoke(this);
    }

    public void PressNoNotify()
    {
        if (isPressed) return;
        isPressed = true;
        SetPressedVisuals();
    }

    public void ResetButton()
    {
        isPressed = false;
        SetNormalVisuals();
    }

    private void SetPressedVisuals()
    {
        if (spriteRenderer == null) return;
        if (pressedSprite != null)
            spriteRenderer.sprite = pressedSprite;
        Color color = spriteRenderer.color;
        color.a = pressedAlpha;
        spriteRenderer.color = color;
    }

    private void SetNormalVisuals()
    {
        if (spriteRenderer == null) return;
        if (pressedSprite != null)
            spriteRenderer.sprite = originalSprite;
        Color color = spriteRenderer.color;
        color.a = normalAlpha;
        spriteRenderer.color = color;
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer == null) return;
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
