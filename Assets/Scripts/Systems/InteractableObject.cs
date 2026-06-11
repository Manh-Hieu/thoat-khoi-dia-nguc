using UnityEngine;

namespace EscapeFromHell.Systems
{
    public abstract class InteractableObject : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private string promptMessage = "Tương tác";
        [SerializeField] private float interactionRadius = 1f;

        // Visual cue gameobject (e.g. the floating "E" icon)
        [SerializeField] protected GameObject interactionPromptPrefab;
        protected GameObject activePromptInstance;

        protected virtual void Start()
        {
            // Set up a trigger collider programmatically if not present
            CircleCollider2D trigger = GetComponent<CircleCollider2D>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<CircleCollider2D>();
                trigger.isTrigger = true;
                trigger.radius = interactionRadius;
            }
        }

        public string PromptMessage => promptMessage;

        // Abstract method to be implemented by child classes (e.g. door, laptop, chest)
        public abstract void Interact();

        public virtual void ShowPrompt(Transform playerTransform)
        {
            if (activePromptInstance != null) return;

            // Instantiate prompt UI if set
            if (interactionPromptPrefab != null)
            {
                activePromptInstance = Instantiate(interactionPromptPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity, transform);
            }
            else
            {
                // Fallback or debug log
                // Debug.Log($"Near interactable: {gameObject.name}. Press E to: {promptMessage}");
            }
        }

        public virtual void HidePrompt()
        {
            if (activePromptInstance != null)
            {
                Destroy(activePromptInstance);
                activePromptInstance = null;
            }
        }
    }
}
