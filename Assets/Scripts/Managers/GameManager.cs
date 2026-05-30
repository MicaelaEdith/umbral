using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentBuildingName;
    public Waypoint currentDestinationWaypoint;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateLocation()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        if (drawer != null && currentDestinationWaypoint != null)
        {
            drawer.CalculatePath(currentDestinationWaypoint);
        }
    }
}
