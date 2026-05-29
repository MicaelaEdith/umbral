using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private string buildingName;

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnWalk()
    {
        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.UpdateLocation();
        gameObject.SetActive(false);
    }

    public void OnBus()
    {
        GameManager.Instance.currentBuildingName = buildingName;
        GameManager.Instance.UpdateLocation();
        gameObject.SetActive(false);
    }
}
