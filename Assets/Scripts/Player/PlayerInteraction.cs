using UnityEngine;
using System.Collections.Generic;
using EscapeFromHell.Systems;
using EscapeFromHell.Core;

namespace EscapeFromHell.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private List<InteractableObject> interactablesInRange = new List<InteractableObject>();
        private InteractableObject closestInteractable;

        private void Update()
        {
            // Only process interaction if the game is in the Playing state
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            {
                if (closestInteractable != null)
                {
                    closestInteractable.HidePrompt();
                    closestInteractable = null;
                }
                return;
            }

            FindClosestInteractable();

            if (closestInteractable != null && Input.GetKeyDown(interactKey))
            {
                closestInteractable.Interact();
            }

            // Right click to interact
            if (Input.GetMouseButtonDown(1))
            {
                DetectMouseClick();
            }
        }

        private void DetectMouseClick()
        {
            if (Camera.main == null) return;

            // Prevent interaction clicking if the dialogue system is active
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            {
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] hitColliders = Physics2D.OverlapPointAll(mousePosition);
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider != null)
                {
                    InteractableObject interactable = hitCollider.GetComponent<InteractableObject>();
                    if (interactable != null)
                    {
                        // Allow clicking if player is close (within 1.5 units)
                        float distance = Vector2.Distance(transform.position, interactable.transform.position);
                        if (distance <= 1.5f)
                        {
                            interactable.Interact();
                            break; // Interact with the first valid interactable found
                        }
                    }
                }
            }
        }

        private void FindClosestInteractable()
        {
            if (interactablesInRange.Count == 0)
            {
                if (closestInteractable != null)
                {
                    closestInteractable.HidePrompt();
                    closestInteractable = null;
                }
                return;
            }

            InteractableObject newClosest = null;
            float minDistance = float.MaxValue;

            for (int i = interactablesInRange.Count - 1; i >= 0; i--)
            {
                var interactable = interactablesInRange[i];
                if (interactable == null || !interactable.gameObject.activeInHierarchy)
                {
                    interactablesInRange.RemoveAt(i);
                    continue;
                }

                float distance = Vector2.Distance(transform.position, interactable.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    newClosest = interactable;
                }
            }

            if (newClosest != closestInteractable)
            {
                if (closestInteractable != null)
                {
                    closestInteractable.HidePrompt();
                }

                closestInteractable = newClosest;

                if (closestInteractable != null)
                {
                    closestInteractable.ShowPrompt(transform);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            InteractableObject interactable = other.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (!interactablesInRange.Contains(interactable))
                {
                    interactablesInRange.Add(interactable);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            InteractableObject interactable = other.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                if (interactablesInRange.Contains(interactable))
                {
                    interactablesInRange.Remove(interactable);
                    if (closestInteractable == interactable)
                    {
                        interactable.HidePrompt();
                        closestInteractable = null;
                    }
                }
            }
        }
    }
}
