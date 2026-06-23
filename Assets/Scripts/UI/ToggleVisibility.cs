using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    public void Show()
    {
        if (target != null)
            target.SetActive(true);
    }

    public void Hide()
    {
        if (target != null)
            target.SetActive(false);
    }
}
