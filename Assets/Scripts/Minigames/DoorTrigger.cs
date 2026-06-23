using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject minigame;
    [SerializeField] private GameObject[] objectsToHideOnOpen;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.15f);

    private SpriteRenderer sr;
    private Color defaultColor;
    private bool wasMinigameActive;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
        if (minigame != null)
            wasMinigameActive = minigame.activeSelf;
    }

    private void Update()
    {
        if (minigame == null) return;

        bool isActive = minigame.activeSelf;
        if (isActive == wasMinigameActive) return;
        wasMinigameActive = isActive;

        foreach (var obj in objectsToHideOnOpen)
            if (obj != null) obj.SetActive(!isActive);
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = defaultColor;
    }

    private void OnMouseDown()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (minigame != null)
        {
            minigame.SetActive(true);
            if (GameManager.Instance != null)
                GameManager.Instance.PushSubLocation(minigame);
        }
    }
}
