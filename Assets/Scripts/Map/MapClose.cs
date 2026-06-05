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
        MapToggle.IsOpen = false;
        mapView.SetActive(false);
        closeButton.SetActive(false);
        openButton.SetActive(true);
        player.SetActive(true);

        Card[] cards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card card in cards)
            card.Close();
    }
}
