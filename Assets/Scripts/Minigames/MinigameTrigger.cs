using UnityEngine;

public class MinigameTrigger : MonoBehaviour
{
    [SerializeField] private GameObject minigameObject;
    [SerializeField] private bool canActivate = true;
    [SerializeField] private bool hoverWhenDisabled = false;
    [SerializeField] private GameObject mapButton;
    [SerializeField] private GameObject npcToHide;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.5f);

    public static int ActiveCount { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool wasMinigameActive;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (minigameObject == null) return;

        bool isActive = minigameObject.activeSelf;
        if (isActive == wasMinigameActive) return;

        wasMinigameActive = isActive;
        if (!isActive) ActiveCount--;

        if (npcToHide != null)
            npcToHide.SetActive(!isActive);

        if (mapButton != null)
            mapButton.SetActive(!isActive);

        if (spriteRenderer != null)
        {
            bool showSprite = (canActivate || hoverWhenDisabled) && !isActive;
            spriteRenderer.enabled = showSprite;
            if (showSprite)
                spriteRenderer.color = originalColor;
        }
    }

    public void SetCanActivate(bool value)
    {
        canActivate = value;
        if (spriteRenderer != null)
        {
            bool showSprite = (canActivate || hoverWhenDisabled) && (minigameObject == null || !minigameObject.activeSelf);
            spriteRenderer.enabled = showSprite;
        }
    }

    public void SetMinigameObject(GameObject newMinigame)
    {
        minigameObject = newMinigame;
        wasMinigameActive = !(minigameObject != null && minigameObject.activeSelf);
    }

    public void NotifyClosed()
    {
        if (mapButton != null)
            mapButton.SetActive(true);
    }

    private void OnMouseDown()
    {
        if (!canActivate || MapToggle.IsOpen) return;
        if (minigameObject == null) return;
        if (minigameObject.activeSelf) return;
        if (mapButton != null)
            mapButton.SetActive(false);
        minigameObject.SetActive(true);
        ActiveCount++;
    }

    private void OnMouseEnter()
    {
        if (MapToggle.IsOpen) return;
        if (spriteRenderer == null) return;
        if (minigameObject != null && minigameObject.activeSelf) return;
        if (!canActivate && !hoverWhenDisabled) return;
        spriteRenderer.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = originalColor;
    }
}
