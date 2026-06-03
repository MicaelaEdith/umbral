using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentBuildingName;
    public Waypoint currentDestinationWaypoint;
    public string currentLocation;

    [SerializeField] private BuildingInterior[] allBuildings;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLocation = "house";

        foreach (var building in allBuildings)
        {
            if (building.BuildingName == currentLocation)
                building.Show();
            else
                building.Hide();
        }
    }

    public void UpdateLocation()
    {
        currentLocation = currentBuildingName;

        foreach (var building in allBuildings)
        {
            if (building.BuildingName == currentLocation)
                building.Show();
            else
                building.Hide();
        }
    }
}
