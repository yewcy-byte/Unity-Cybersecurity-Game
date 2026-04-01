using TMPro;
using UnityEngine;
using System.Collections;
using System;

public class GuiderBot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private float typingSpeed = 0.05f;

    private int dialogueIndex;
    private bool isTyping;
    private int startLine;
    public float waitNextLine = 1.5f;
private int endLine;
    private Action onDialogueRangeComplete;

public void StartDialogueRangeDelayed(int start, int end, float delay)
{
    StartCoroutine(StartDialogueAfterDelay(start, end, delay));
}



IEnumerator StartDialogueAfterDelay(int start, int end, float delay)
{
    yield return new WaitForSeconds(delay);
    StartDialogueRange(start, end);
}


public void StartDialogueRange(int start, int end)
{
    StartDialogueRange(start, end, null);
}

public void StartDialogueRange(int start, int end, Action onComplete)
{
    startLine = Mathf.Clamp(start, 0, dialogueLines.Length - 1);
    endLine = Mathf.Clamp(end, startLine, dialogueLines.Length - 1);
    onDialogueRangeComplete = onComplete;

    dialogueIndex = startLine;

    StopAllCoroutines();
    StartCoroutine(TypeLine());
}


IEnumerator TypeLine()
{
    isTyping = true;
    dialogueText.text = "";

    foreach (char letter in dialogueLines[dialogueIndex])
    {
        dialogueText.text += letter;
        SoundManager.Play("Dialogue");
        yield return new WaitForSeconds(typingSpeed);
    }

    isTyping = false;

    yield return new WaitForSeconds(waitNextLine); 
    NextLine();
}



public void NextLine()
{
    if (isTyping)
    {
        StopAllCoroutines();
        dialogueText.text = dialogueLines[dialogueIndex];
        isTyping = false;
        return;
    }

    dialogueIndex++;

    if (dialogueIndex <= endLine)
    {
        StartCoroutine(TypeLine());
    }
    else
    {
        dialogueText.text = dialogueLines[endLine];
        onDialogueRangeComplete?.Invoke();
        onDialogueRangeComplete = null;
    }
}

}
