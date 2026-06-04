using UnityEngine;

public class RedirectButton : MonoBehaviour
{
    [SerializeField] private GameObject minigameParent;
    [SerializeField] private GameObject mapButton;

    private void OnMouseDown()
    {
        Relocate();

        if (mapButton != null)
            mapButton.SetActive(true);

        if (minigameParent != null)
            minigameParent.SetActive(false);
    }

    private void Relocate()
    {
    }
}
