using UnityEngine;

public class MapToggle : MonoBehaviour
{
    public static bool IsOpen { get; set; }

    [SerializeField]
    private GameObject mapView;

    [SerializeField]
    private GameObject lbl_day;

    [SerializeField]
    private GameObject closeButton;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject[] npcs;

    public void OpenMap()
    {
        IsOpen = true;
        mapView.SetActive(true);
        closeButton.SetActive(true);
        player.SetActive(false);
        foreach (GameObject npc in npcs)
            npc.SetActive(false);
        gameObject.SetActive(false);

        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null)
            drawer.ClearPath();
    }
}
