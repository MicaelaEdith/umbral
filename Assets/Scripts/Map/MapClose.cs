using UnityEngine;

public class MapClose : MonoBehaviour
{
    [SerializeField]
    private GameObject mapView;

    [SerializeField]
    private GameObject lbl_day;


    [SerializeField]
    private GameObject openButton;

    [SerializeField]
    private GameObject closeButton;

    [SerializeField]
    private GameObject player;

    public void Close()
    {
        mapView.SetActive(false);
        closeButton.SetActive(false);
        openButton.SetActive(true);
        player.SetActive(true);
        lbl_day.SetActive(true);
    }
}
