using UnityEngine;

public class HospitalRedirect : MonoBehaviour
{
    [SerializeField] private GameObject targetRoom;
    [SerializeField] private GameObject previousRoom;
    [SerializeField] private int requiredProgress = 1;
    [SerializeField] private Color hoverColor = new Color(0f, 0f, 0f, 0.2f);

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
    }

    private void OnMouseEnter()
    {
        if (sr != null)
            sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
    }

    private void OnMouseDown()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.questProgress < requiredProgress) return;
        if (targetRoom == null) return;

        targetRoom.SetActive(true);
        GameManager.Instance.PushSubLocation(targetRoom, previousRoom);
    }
}
