using UnityEngine;

public class SignSection : MonoBehaviour
{
    [SerializeField]
    private Color hoverColor = new Color(1, 1, 1, 0.5f);

    private SpriteRenderer sr;
    private Color defaultColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (sr != null)
            sr.color = defaultColor;
    }

    private void Start()
    {
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
}
