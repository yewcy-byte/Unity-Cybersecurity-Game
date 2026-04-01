using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private AudioClip MissionMusic;
    [SerializeField] private AudioClip brainTimeMusic;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.6f;
    [SerializeField] private bool playDefaultOnStart = true;

    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
    }

    private void Start()
    {
        if (playDefaultOnStart)
        {
            PlayMusic(defaultMusic);
        }
    }

    public static void SwitchToBrainTimeMusic()
    {
        if (Instance == null)
        {
            return;
        }

        Instance.PlayMusic(Instance.brainTimeMusic);
    }

    public static void SwitchToDefaultMusic()
    {
        if (Instance == null)
        {
            return;
        }

        Instance.PlayMusic(Instance.defaultMusic);
    }

      public static void SwitchToMissionMusic()
    {
        if (Instance == null)
        {
            return;
        }

        Instance.PlayMusic(Instance.MissionMusic);
    }

    public static void SetMusicVolume(float volume)
    {
        if (Instance == null || Instance.musicSource == null)
        {
            return;
        }

        Instance.musicVolume = Mathf.Clamp01(volume);
        Instance.musicSource.volume = Instance.musicVolume;
    }

    public static void SetMuted(bool muted)
    {
        if (Instance == null || Instance.musicSource == null)
        {
            return;
        }

        Instance.musicSource.mute = muted;
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }
}
