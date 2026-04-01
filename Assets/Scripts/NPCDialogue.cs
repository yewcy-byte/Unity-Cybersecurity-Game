using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public float typingSpeed = 2f;

    public float voicePitch = 1f;

    public bool[] autoProgressLines;
    public bool[] endDialogueLines;

    public float autoProgressDelay = 8f;

        public AudioClip voiceClip;   // 👈 ADD THIS

        public AudioClip Goodluck;

        public DialogueChoice[] choices ;

        public int questInProgressIndex;
        public int questCompletedIndex;
        public List<Mission> missions;
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndexes;

    public AudioClip[] OptionsAudio;
    public bool[] givesQuest;

}
