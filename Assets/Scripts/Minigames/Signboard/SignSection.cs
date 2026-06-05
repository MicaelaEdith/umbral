using UnityEngine;

public class SignSection : MonoBehaviour
{
    [SerializeField] private Color hoverColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] private Color pressColor = new Color(1, 1, 1, 0.8f);

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
        sr.color = pressColor;
    }

    private void OnMouseUp()
    {
        sr.color = hoverColor;
    }
}
