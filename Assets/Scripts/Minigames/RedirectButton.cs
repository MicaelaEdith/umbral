using UnityEngine;

public class RedirectButton : MonoBehaviour
{
    [SerializeField] private GameObject minigameParent;
    [SerializeField] private GameObject mapButton;
    [SerializeField] private SignboardDirector director;
    [SerializeField] private int optionIndex;

    private void OnMouseDown()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (director != null)
            director.SelectOption(optionIndex);

        if (mapButton != null)
            mapButton.SetActive(true);

        if (minigameParent != null)
            minigameParent.SetActive(false);
    }
}
