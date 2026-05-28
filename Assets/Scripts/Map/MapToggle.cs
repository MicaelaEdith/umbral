using UnityEngine;

public class MapToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject mapView;

    [SerializeField]
    private GameObject closeButton;

    [SerializeField]
    private GameObject player;

    public void OpenMap()
    {
        mapView.SetActive(true);
        closeButton.SetActive(true);
        player.SetActive(false);
        gameObject.SetActive(false);
    }
}
