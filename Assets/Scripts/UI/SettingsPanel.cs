using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI audioStatusText;

    private void Start()
    {
        UpdateAudioText();
    }

    public void Open()
    {
        if (panel != null) panel.SetActive(true);
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void ToggleAudio()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleMute();
        UpdateAudioText();
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClick();
    }

    private void UpdateAudioText()
    {
        if (audioStatusText != null && AudioManager.Instance != null)
            audioStatusText.text = AudioManager.Instance.IsMuted ? "Desactivado" : "Activado";
    }
}
