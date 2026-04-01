using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGame : MonoBehaviour
{
    [SerializeField] private string loginPageUrl = "/index.html";
    private bool restartInProgress;

    private const string FirebaseSaveJsonField = "unityGameSaveJson";
    private const string FirebaseSaveObjectField = "unityGameSaveObject";

    public GameObject AreYouSurePanel;

    // For WebGL, "quit" should redirect to your hosted login page.
    public void QuitGame()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("QuitGame called - attempting to redirect to: " + loginPageUrl);
        if (!string.IsNullOrWhiteSpace(loginPageUrl))
        {
            // Construct absolute URL by getting the base URL of the current page
            string baseUrl = new System.Uri(Application.absoluteURL).GetLeftPart(System.UriPartial.Authority);
            string absoluteUrl = baseUrl + loginPageUrl;
            Debug.Log("Redirecting to: " + absoluteUrl);
            Application.ExternalEval("window.location.href = '" + absoluteUrl + "';");
        }
        else
        {
            Debug.LogError("loginPageUrl is empty or null!");
        }
#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }


    public void AreYouSure()
    {
        AreYouSurePanel.SetActive(true);
    }

    public void RestartGame()
    {
        if (restartInProgress)
        {
            return;
        }

        StartCoroutine(RestartGameRoutine());
    }

    private IEnumerator RestartGameRoutine()
    {
        restartInProgress = true;

        savecontroller save = savecontroller.Instance;
        if (save == null)
        {
            ReloadCurrentScene();
            yield break;
        }

        string userId = save.ActiveUserId;
        string projectId = save.FirebaseProjectId;
        string apiKey = save.FirebaseApiKey;

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(apiKey))
        {
            ReloadCurrentScene();
            yield break;
        }

        string escapedUserId = UnityWebRequest.EscapeURL(userId);
        string escapedSaveJsonField = UnityWebRequest.EscapeURL(FirebaseSaveJsonField);
        string escapedSaveObjectField = UnityWebRequest.EscapeURL(FirebaseSaveObjectField);

        string url =
            "https://firestore.googleapis.com/v1/projects/" + projectId +
            "/databases/(default)/documents/users/" + escapedUserId +
            "?key=" + apiKey +
            "&updateMask.fieldPaths=" + escapedSaveObjectField +
            "&updateMask.fieldPaths=" + escapedSaveJsonField;

        // Set both save fields to null so load path creates fresh new-user save data.
        string requestBody =
            "{\"fields\":{\"" + FirebaseSaveObjectField + "\":{\"nullValue\":null},\"" + FirebaseSaveJsonField + "\":{\"nullValue\":null}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Failed to clear Firestore save data before restart: " + request.error);
            }
        }

        ReloadCurrentScene();
    }

    private void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    
}
