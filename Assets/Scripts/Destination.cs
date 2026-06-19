using UnityEngine;

public class Destination : MonoBehaviour
{
    public static bool blockUnlessInList;
    public static string[] allowedNames;

    [SerializeField]
    private Waypoint destinationWaypoint;

    [SerializeField]
    private GameObject card;

    [SerializeField]
    private float delayBeforeShow = 0.2f;

    private void OnMouseDown()
    {
        if (blockUnlessInList && allowedNames != null)
        {
            bool found = false;
            Card cardComponent = card != null ? card.GetComponent<Card>() : null;
            string id = cardComponent != null ? cardComponent.BuildingName : gameObject.name;
            foreach (string name in allowedNames)
            {
                if (id == name) { found = true; break; }
            }
            if (!found) return;
        }

        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null)
        {
            drawer.CalculatePath(destinationWaypoint);
        }
        Invoke(nameof(ShowCard), delayBeforeShow);
    }

    private void ShowCard()
    {
        if (card == null) return;

        Card[] allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card c in allCards)
        {
            if (c.gameObject != card)
                c.Close();
        }

        card.SetActive(true);
    }
}