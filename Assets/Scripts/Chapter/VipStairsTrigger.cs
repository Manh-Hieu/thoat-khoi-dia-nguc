using UnityEngine;
using EscapeFromHell.Player;
using EscapeFromHell.Systems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EscapeFromHell.Chapter
{
    public class VipStairsTrigger : MonoBehaviour
    {
        private bool isChecking = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !isChecking)
            {
                if (Chapter2Controller.Instance != null && Chapter2Controller.Instance.CurrentCash < 10000000)
                {
                    StartCoroutine(HandleBlockSequence(other.gameObject));
                }
            }
        }

        private IEnumerator HandleBlockSequence(GameObject playerObj)
        {
            isChecking = true;

            // Get Player components
            var playerController = playerObj.GetComponent<PlayerController>();
            var rb = playerObj.GetComponent<Rigidbody2D>();
            var animator = playerObj.GetComponent<Animator>();

            // 1. Stop and disable controls
            if (playerController != null)
            {
                playerController.SetControlEnabled(false);
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }

            // 2. Play dialogue
            if (DialogueSystem.Instance != null)
            {
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Đứng lại! Tầng 2 là khu vực VIP dành riêng cho giới siêu giàu." },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Tài khoản của cậu phải có ít nhất 10.000.000đ mới được phép bước lên đây." },
                    new DialogueLine { speakerName = "Minh", text = "10.000.000đ sao... Mình chưa có đủ tiền." },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Vậy thì xin mời cậu xuống dưới chơi tiếp, khi nào đủ tiền hãy quay lại." }
                };

                bool dialogueFinished = false;
                Action onEnd = null;
                onEnd = () => {
                    dialogueFinished = true;
                    DialogueSystem.Instance.OnDialogueEnd -= onEnd;
                };
                DialogueSystem.Instance.OnDialogueEnd += onEnd;
                DialogueSystem.Instance.StartDialogue(d);

                yield return new WaitUntil(() => dialogueFinished);
            }

            // 3. Walk the player back down a little bit (down-right)
            Vector2 startPos = playerObj.transform.position;
            Vector2 targetPos = startPos + new Vector2(1.2f, -0.8f);
            
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
                Vector2 dir = new Vector2(1.2f, -0.8f).normalized;
                animator.SetFloat("MoveX", dir.x);
                animator.SetFloat("MoveY", dir.y);
                animator.SetFloat("LastMoveX", dir.x);
                animator.SetFloat("LastMoveY", dir.y);
            }

            float duration = 0.6f; // Time to walk back down a little bit
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Vector2 currentPos = Vector2.Lerp(startPos, targetPos, elapsed / duration);
                if (rb != null)
                {
                    rb.MovePosition(currentPos);
                }
                else
                {
                    playerObj.transform.position = currentPos;
                }
                yield return null;
            }

            if (rb != null) rb.position = targetPos;
            else playerObj.transform.position = targetPos;

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                // Face the stairs guard (left-ish)
                animator.SetFloat("LastMoveX", -1.0f);
                animator.SetFloat("LastMoveY", 0.0f);
            }

            // 4. Re-enable control
            if (playerController != null)
            {
                playerController.SetControlEnabled(true);
            }

            // Short cooldown to prevent re-triggering immediately
            yield return new WaitForSeconds(0.5f);
            isChecking = false;
        }
    }
}
