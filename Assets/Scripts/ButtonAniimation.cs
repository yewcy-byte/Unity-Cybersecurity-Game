using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    public void ButtonPop()
    {
        LeanTween.scale(gameObject, new Vector3(0.9f, 0.9f, 1f), 0.1f)
            .setEase(LeanTweenType.easeOutCirc);

        LeanTween.scale(gameObject, Vector3.one, 0.15f)
            .setDelay(0.1f)
            .setEase(LeanTweenType.easeOutCirc);
    }


      public void ButtonScaleDown()
    {
       
        LeanTween.scale(gameObject, new Vector3(0f,0f,0f), 0.15f)
            .setDelay(0.1f)
            .setEase(LeanTweenType.easeOutCirc);
            
    }
}
