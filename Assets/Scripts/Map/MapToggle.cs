using UnityEngine;

public class MapToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject mapView;

    [SerializeField]
    private GameObject lbl_day;

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

        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null)
            drawer.ClearPath();
    }
}
