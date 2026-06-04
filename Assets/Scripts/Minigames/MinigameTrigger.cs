using UnityEngine;

public class MinigameTrigger : MonoBehaviour
{
    [SerializeField] private GameObject minigameObject;
    [SerializeField] private bool canActivate = true;
    [SerializeField] private GameObject mapButton;

    public void SetCanActivate(bool value)
    {
        canActivate = value;
    }

    public void NotifyClosed()
    {
        if (mapButton != null)
            mapButton.SetActive(true);
    }

    private void OnMouseDown()
    {
        if (!canActivate || minigameObject == null) return;
        if (mapButton != null)
            mapButton.SetActive(false);
        minigameObject.SetActive(true);
    }
}
