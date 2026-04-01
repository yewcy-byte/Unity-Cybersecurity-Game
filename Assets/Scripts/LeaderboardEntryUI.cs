using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text timeText;

    public void SetData(int rank, string username, float totalSeconds, bool isCurrentUser = false)
    {
        rankText.text = $"#{rank}";
        usernameText.text = isCurrentUser ? $"{username} (You)" : username;
        timeText.text = FormatSeconds(totalSeconds);
    }

    private string FormatSeconds(float totalSeconds)
    {
        int h = (int)(totalSeconds / 3600);
        int m = (int)(totalSeconds % 3600 / 60);
        int s = (int)(totalSeconds % 60);
        return h > 0 ? $"{h}h {m:00}m {s:00}s" : $"{m:00}m {s:00}s";
    }
}
