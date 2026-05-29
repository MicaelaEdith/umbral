using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField]
    private Waypoint destinationWaypoint;

    [SerializeField]
    private GameObject card;

    [SerializeField]
    private float delayBeforeShow = 0.2f;

    private void OnMouseDown()
    {
        Invoke(nameof(ShowCard), delayBeforeShow);
    }

    private void ShowCard()
    {
        if (card != null)
            card.SetActive(true);
    }
}