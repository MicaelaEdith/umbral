using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField]
    private Waypoint destinationWaypoint;

    private void OnMouseDown()
    {
        PathDrawer drawer = FindFirstObjectByType<PathDrawer>();
        drawer.CalculatePath(destinationWaypoint);
    }
}