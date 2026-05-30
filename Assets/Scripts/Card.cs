using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private string buildingName;

    [SerializeField]
    private Waypoint destinationWaypoint;

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnWalk()
    {
        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.currentDestinationWaypoint = destinationWaypoint;
        GameManager.Instance.UpdateLocation();
        gameObject.SetActive(false);
    }

    public void OnBus()
    {
        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.currentDestinationWaypoint = destinationWaypoint;
        GameManager.Instance.UpdateLocation();
        gameObject.SetActive(false);
    }
}
