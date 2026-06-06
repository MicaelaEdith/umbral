using UnityEngine;

public class DoorSelector : MonoBehaviour
{
    [SerializeField] private bool isCorrect;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.2f);

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
        if (!isCorrect && GameManager.Instance != null)
        {
            GameManager.Instance.ScheduleTime(2);
            GameManager.Instance.SetShameTimed(0.25f, 10);
        }

        sr.color = defaultColor;
    }
}
