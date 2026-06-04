using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField]
    private string buildingName;

    [SerializeField]
    private Waypoint destinationWaypoint;

    [SerializeField]
    private TextMeshProUGUI walkTimeLabel;

    [SerializeField]
    private TextMeshProUGUI walkCostLabel;

    [SerializeField]
    private TextMeshProUGUI busTimeLabel;

    [SerializeField]
    private TextMeshProUGUI busCostLabel;

    private void OnEnable()
    {
        UpdateTravelInfo();
    }

    private void UpdateTravelInfo()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer == null) return;

        drawer.CalculateTravelOptions(drawer.TotalDistance, out int walkTime, out int busTime, out int busCost);

        if (walkTimeLabel != null)
            walkTimeLabel.text = $"{walkTime} min";
        if (walkCostLabel != null)
            walkCostLabel.text = "Gratis";
        if (busTimeLabel != null)
            busTimeLabel.text = $"{busTime} min";
        if (busCostLabel != null)
            busCostLabel.text = $"$ {busCost}";
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnWalk()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null)
        {
            drawer.CalculateTravelOptions(drawer.TotalDistance, out int walkTime, out int _, out int _);
            GameManager.Instance.ScheduleMinutes(walkTime);
            drawer.SetCurrentWaypoint(destinationWaypoint);
        }

        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.currentDestinationWaypoint = destinationWaypoint;
        GameManager.Instance.UpdateLocation();

        CloseCardAndMap();
    }

    public void OnBus()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null)
        {
            drawer.CalculateTravelOptions(drawer.TotalDistance, out int _, out int busTime, out int busCost);

            if (!GameManager.Instance.CanAfford(busCost))
                return;

            GameManager.Instance.SpendMoney(busCost);
            GameManager.Instance.ScheduleMinutes(busTime);
            drawer.SetCurrentWaypoint(destinationWaypoint);
        }

        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.currentDestinationWaypoint = destinationWaypoint;
        GameManager.Instance.UpdateLocation();

        CloseCardAndMap();
    }

    private void CloseCardAndMap()
    {
        gameObject.SetActive(false);

        MapClose mapClose = FindFirstObjectByType<MapClose>();
        if (mapClose != null)
            mapClose.Close();
    }
}
