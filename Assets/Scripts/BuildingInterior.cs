using UnityEngine;

public class BuildingInterior : MonoBehaviour
{
    [SerializeField]
    private string buildingName;

    public string BuildingName => buildingName;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
