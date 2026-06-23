using UnityEngine;

public class Ticket : MonoBehaviour
{
    public event System.Action OnCollected;

    private void OnMouseDown()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        OnCollected?.Invoke();
    }
}
