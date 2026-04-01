using UnityEngine;

public class MissionOpenButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public GameObject OpenThis ;

  public GameObject playerHint;

    public void OpenPanel()
    {
        LeanTween.scale(playerHint, new Vector3(0, 0, 0), 0.3f)
            .setEase(LeanTweenType.easeInOutBack);
            
        OpenThis.SetActive(!OpenThis.activeSelf);

    }
}
