using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentBuildingName;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateLocation()
    {
        // TODO: implementar
    }
}
