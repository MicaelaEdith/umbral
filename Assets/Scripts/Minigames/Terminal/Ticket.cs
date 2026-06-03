using UnityEngine;

public class Ticket : MonoBehaviour
{
    public event System.Action OnCollected;

    private void OnMouseDown()
    {
        OnCollected?.Invoke();
    }
}
