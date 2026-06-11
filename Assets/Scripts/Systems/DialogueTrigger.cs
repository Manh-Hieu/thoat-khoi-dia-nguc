using UnityEngine;

namespace EscapeFromHell.Systems
{
    public class DialogueTrigger : InteractableObject
    {
        [Header("Dialogue settings")]
        [SerializeField] private DialogueData dialogueData;
        [SerializeField] private bool triggerOnEnter = false;
        [SerializeField] private bool triggerOnce = false;

        private bool hasTriggered = false;

        public void TriggerDialogue()
        {
            if (triggerOnce && hasTriggered) return;

            if (DialogueSystem.Instance != null && dialogueData != null)
            {
                hasTriggered = true;
                DialogueSystem.Instance.StartDialogue(dialogueData);
            }
        }

        public override void Interact()
        {
            if (!triggerOnEnter)
            {
                TriggerDialogue();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Call parent OnTriggerEnter2D to handle interaction prompt detection
            base.Start(); // Ensure collider is initialized if enter happens very early

            // Also check if triggerOnEnter is enabled and it was hit by the Player
            if (triggerOnEnter && other.CompareTag("Player"))
            {
                TriggerDialogue();
            }
        }
    }
}
