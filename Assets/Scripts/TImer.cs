using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour 
{

    public static Timer Instance;
    [SerializeField] TMP_Text timertext;
    float elapsedTime;
    bool isTimerRunning = false; 
    private const float DuplicateDeductionWindowSeconds = 0.2f;
    private float lastDeductionUnscaledTime = -999f;
    private float lastDeductionSeconds = -1f;

void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timertext.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    public void DeductTime(float seconds)
    {
        float safeSeconds = Mathf.Abs(seconds);
        if (safeSeconds <= 0f)
        {
            return;
        }

        // Ignore accidental duplicate calls that happen in the same instant.
        if (Mathf.Approximately(lastDeductionSeconds, safeSeconds)
            && (Time.unscaledTime - lastDeductionUnscaledTime) <= DuplicateDeductionWindowSeconds)
        {
            return;
        }

        lastDeductionSeconds = safeSeconds;
        lastDeductionUnscaledTime = Time.unscaledTime;

        elapsedTime -= safeSeconds;
        if (elapsedTime < 0)
        {
            elapsedTime = 0;
        }
    }

public float GetTimeData()
{
    return elapsedTime;
}

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
        timertext.text = "00:00";
    }
}