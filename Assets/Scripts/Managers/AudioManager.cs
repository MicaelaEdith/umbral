using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickClip;

    [SerializeField] private AudioClip musicClip;

    private bool isMuted;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyMute();
    }

    private void Start()
    {
        if (musicClip == null)
            musicClip = Resources.Load<AudioClip>("Audio/ashes_Dm");
        if (clickClip == null)
            clickClip = Resources.Load<AudioClip>("Audio/click");

        if (musicClip == null)
        {
            Debug.LogWarning("AudioManager: No se pudo cargar ashes_Dm. Creando clip silencioso.");
            musicClip = CreateSilentClip();
        }
        if (clickClip == null)
        {
            Debug.LogWarning("AudioManager: No se pudo cargar click. Creando clip silencioso.");
            clickClip = CreateSilentClip();
        }

        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            if (musicSource.playOnAwake)
                musicSource.Play();
        }

        ApplyMute();
    }

    private static AudioClip CreateSilentClip()
    {
        AudioClip clip = AudioClip.Create("SilentClip", 44100, 1, 44100, false);
        return clip;
    }

    public void PlayClick()
    {
        if (!isMuted && sfxSource != null && clickClip != null)
            sfxSource.PlayOneShot(clickClip);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyMute();
    }

    public bool IsMuted => isMuted;

    private void ApplyMute()
    {
        if (musicSource != null) musicSource.mute = isMuted;
        if (sfxSource != null) sfxSource.mute = isMuted;
    }
}
