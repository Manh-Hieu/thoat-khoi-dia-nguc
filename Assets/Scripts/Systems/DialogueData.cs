using UnityEngine;
using System.Collections.Generic;

namespace EscapeFromHell.Systems
{
    [System.Serializable]
    public struct DialogueChoice
    {
        [TextArea(1, 2)] public string choiceText;
        public DialogueData nextDialogue; // Dialogue to transition to if chosen
    }

    [System.Serializable]
    public struct DialogueLine
    {
        public string speakerName;
        public Sprite speakerPortrait;
        [TextArea(3, 5)] public string text;
    }

    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Escape From Hell/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        public List<DialogueLine> lines;
        public List<DialogueChoice> choices; // Optional branching choices at the end
    }
}
