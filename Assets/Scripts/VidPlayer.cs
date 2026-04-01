using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

public void PlayVideo(string videoFileName)
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();

        // Stop and reset the video player to clear previous state
        videoPlayer.Stop();

        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        Debug.Log("Video Path: " + videoPath);
        videoPlayer.url = videoPath;
        videoPlayer.Play();
    }
}
