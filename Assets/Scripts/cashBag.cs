using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class cashBag : MonoBehaviour
{
    [Header("Sequence References")]
    [SerializeField] private GameObject cage;
    public VidPlayer vidPlayer;
    [SerializeField] private RawImage videoRawImage;

    [Header("Timing")]
    [SerializeField] private float delayBeforeVideo = 3f;
    [SerializeField] private float videoFadeInDuration = 0.25f;
    [SerializeField] private float videoFadeOutDuration = 0.35f;

    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";


    [Header("Missions")]
    public List<Mission> missions;

    public GameObject RedTeamStations;
    public GameObject BlueTeamStations;

    private enum QuestState {NotStarted, InProgress, Completed}
    private QuestState questState = QuestState.NotStarted;




    private bool hasTriggered;
    private bool videoFinished;

    private void Awake()
    {
        if (cage != null)
        {
            cage.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.Play("HeavyMetal");

        if (hasTriggered)
        {
            return;
        }

        if (!collision.CompareTag(playerTag))
        {
            return;
        }

        hasTriggered = true;

        Collider2D ownCollider = GetComponent<Collider2D>();
        if (ownCollider != null)
        {
            ownCollider.enabled = false;
        }

           foreach (Mission m in missions)
{
    MissionController.Instance.AcceptMission(m);
    questState = QuestState.InProgress;
}

        savecontroller.Instance?.SaveGame();

        
        
        StartCoroutine(TrapSequence());
    }

    private System.Collections.IEnumerator TrapSequence()
    {

        PauseController.SetPause(true);
        BackgroundMusicManager.SetMuted(true);
        SoundManager.SetMuted(true);



        if (cage != null)
        {
            cage.SetActive(true);
        }

        if (videoRawImage != null)
        {
            Color startColor = videoRawImage.color;
            startColor.a = 0f;
            videoRawImage.color = startColor;
        }

        yield return new WaitForSecondsRealtime(delayBeforeVideo);

        if (vidPlayer != null)
        {
            videoFinished = false;
            VideoPlayer vp = vidPlayer.GetComponent<VideoPlayer>();
            if (vp != null)
            {
                vp.loopPointReached += OnVideoFinished;
                vp.errorReceived += OnVideoError;
            }

            vidPlayer.PlayVideo("catch.mp4");
            yield return StartCoroutine(FadeInVideoRawImage());

            while (!videoFinished)
            {
                yield return null;
            }

            if (vp != null)
            {
                vp.loopPointReached -= OnVideoFinished;
                vp.errorReceived -= OnVideoError;
            }
        }

        yield return StartCoroutine(FadeOutVideoRawImage());

        if (cage != null)
        {
            cage.SetActive(false);
        }
        BlueTeamStations.SetActive(true);
        RedTeamStations.SetActive(false);
        BackgroundMusicManager.SetMuted(false);
        SoundManager.SetMuted(false);
        PauseController.SetPause(false);
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FadeOutVideoRawImage()
    {
        if (videoRawImage == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Color currentColor = videoRawImage.color;
        float startingAlpha = currentColor.a;

        while (elapsed < videoFadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / videoFadeOutDuration);
            currentColor.a = Mathf.Lerp(startingAlpha, 0f, t);
            videoRawImage.color = currentColor;
            yield return null;
        }

        currentColor.a = 0f;
        videoRawImage.color = currentColor;
    }

    private System.Collections.IEnumerator FadeInVideoRawImage()
    {
        if (videoRawImage == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Color currentColor = videoRawImage.color;
        float startingAlpha = currentColor.a;

        while (elapsed < videoFadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / videoFadeInDuration);
            currentColor.a = Mathf.Lerp(startingAlpha, 1f, t);
            videoRawImage.color = currentColor;
            yield return null;
        }

        currentColor.a = 1f;
        videoRawImage.color = currentColor;
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        videoFinished = true;
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogWarning("cashBag video playback failed: " + message, this);
        videoFinished = true;
    }

    public void ApplyPostTrapStationState()
    {
        if (BlueTeamStations != null)
        {
            BlueTeamStations.SetActive(true);
        }

        if (RedTeamStations != null)
        {
            RedTeamStations.SetActive(false);
        }
    }

    private void OnDisable()
    {
        BackgroundMusicManager.SetMuted(false);
        SoundManager.SetMuted(false);

        if (vidPlayer != null)
        {
            VideoPlayer vp = vidPlayer.GetComponent<VideoPlayer>();
            if (vp != null)
            {
                vp.loopPointReached -= OnVideoFinished;
                vp.errorReceived -= OnVideoError;
            }
        }
    }
}
