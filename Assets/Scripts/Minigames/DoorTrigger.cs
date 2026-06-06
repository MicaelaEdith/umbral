using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject minigame;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.15f);

    private SpriteRenderer sr;
    private Color defaultColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color;
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
        if (minigame != null)
        {
            minigame.SetActive(true);
            if (GameManager.Instance != null)
                GameManager.Instance.PushSubLocation(minigame);
        }
    }
}
