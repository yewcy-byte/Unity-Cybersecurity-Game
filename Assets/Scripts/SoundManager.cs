using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;

    private static AudioSource audioSource;

        private static AudioSource randomPitchaudioSource;

    private static SoundEffectLibrary soundEffectLibrary;

private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSource = audioSources[0];
            randomPitchaudioSource = audioSources[1];
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
          //  DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(string soundName, bool randomPitch = false)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            if (randomPitch)
            {
                randomPitchaudioSource.pitch= Random.Range(1f, 1.5f);
                randomPitchaudioSource.PlayOneShot(audioClip);
            }

            else
            {
            audioSource.PlayOneShot(audioClip);    
            }
            
        }
    }

    void Start()
    {
    }





    public static void SetMuted(bool muted)
    {
        if (audioSource != null)
        {
            audioSource.mute = muted;
        }

        if (randomPitchaudioSource != null)
        {
            randomPitchaudioSource.mute = muted;
        }
    }
}
