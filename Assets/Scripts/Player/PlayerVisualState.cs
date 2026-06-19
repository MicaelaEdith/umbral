using UnityEngine;

public class PlayerVisualState : MonoBehaviour
{
    public static PlayerVisualState Instance { get; private set; }

    [SerializeField] private GameObject[] visualStates;

    private void Awake()
    {
        Instance = this;
    }

    public void SetState(int index)
    {
        for (int i = 0; i < visualStates.Length; i++)
        {
            if (visualStates[i] != null)
                visualStates[i].SetActive(i == index);
        }
    }
}
