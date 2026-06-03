using UnityEngine;

public class TerminalTrigger : MonoBehaviour
{
    [SerializeField] private GameObject terminalMinigame;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (terminalMinigame != null)
            terminalMinigame.SetActive(true);
    }
}
