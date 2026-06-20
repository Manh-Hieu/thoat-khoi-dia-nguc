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
                float playerY = other.transform.position.y;
                // If player is lower than 1.5f, they are coming from the 1st floor (trying to go up)
                bool isGoingUp = playerY < 1.5f;

                if (Chapter2Controller.Instance != null)
                {
                    int cash = Chapter2Controller.Instance.CurrentCash;

                    if (isGoingUp)
                    {
                        // Going up: requires at least 10,000,000đ
                        if (cash < 10000000)
                        {
                            StartCoroutine(HandleBlockGoingUp(other.gameObject));
                        }
                    }
                    else
                    {
                        // Going down: only allowed if broke (cash <= 0) OR won big (cash > 500,000,000đ)
                        if (cash > 500000000)
                        {
                            // Won big: let them leave and congratulate them
                            StartCoroutine(HandleWonBigPassDialogue(other.gameObject));
                        }
                        else if (cash > 0)
                        {
                            // Still has money but less than 500M: block them!
                            StartCoroutine(HandleBlockGoingDown(other.gameObject));
                        }
                        else
                        {
                            // Broke: let them pass
                            StartCoroutine(HandleBrokePassDialogue(other.gameObject));
                        }
                    }
                }
            }
        }

        private IEnumerator HandleBlockGoingUp(GameObject playerObj)
        {
            isChecking = true;

            var playerController = playerObj.GetComponent<PlayerController>();
            var rb = playerObj.GetComponent<Rigidbody2D>();
            var animator = playerObj.GetComponent<Animator>();

            // Stop controls
            if (playerController != null) playerController.SetControlEnabled(false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsMoving", false);

            // Dialogue
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

            // Walk back down (down-right)
            Vector2 startPos = playerObj.transform.position;
            Vector2 targetPos = startPos + new Vector2(1.2f, -0.8f);
            
            yield return StartCoroutine(WalkToPosition(playerObj, startPos, targetPos, new Vector2(1.2f, -0.8f).normalized));

            if (playerController != null) playerController.SetControlEnabled(true);
            yield return new WaitForSeconds(0.5f);
            isChecking = false;
        }

        private IEnumerator HandleBlockGoingDown(GameObject playerObj)
        {
            isChecking = true;

            var playerController = playerObj.GetComponent<PlayerController>();
            var rb = playerObj.GetComponent<Rigidbody2D>();
            var animator = playerObj.GetComponent<Animator>();

            // Stop controls
            if (playerController != null) playerController.SetControlEnabled(false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsMoving", false);

            // Dialogue
            if (DialogueSystem.Instance != null)
            {
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Này cậu kia! Khu VIP này đâu phải muốn đến là đến, muốn đi là đi được!" },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Trừ khi cậu hết sạch tiền, hoặc có tài sản hơn 500.000.000đ thì tôi mới cho ra." },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Tôi chỉ muốn giúp những kẻ ý chí không vững như cậu có cơ hội làm giàu thôi." },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Mới thua có mấy ván mà đã nản chí rồi sao?" },
                    new DialogueLine { speakerName = "Minh", text = "Nhưng tôi..." },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Không nói nhiều! Quay lại bàn cược chơi tiếp đi." }
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

            // Walk back up (up-left)
            Vector2 startPos = playerObj.transform.position;
            Vector2 targetPos = startPos + new Vector2(-1.2f, 0.8f);
            
            yield return StartCoroutine(WalkToPosition(playerObj, startPos, targetPos, new Vector2(-1.2f, 0.8f).normalized));

            if (playerController != null) playerController.SetControlEnabled(true);
            yield return new WaitForSeconds(0.5f);
            isChecking = false;
        }

        private IEnumerator HandleWonBigPassDialogue(GameObject playerObj)
        {
            isChecking = true;

            var playerController = playerObj.GetComponent<PlayerController>();
            var rb = playerObj.GetComponent<Rigidbody2D>();
            var animator = playerObj.GetComponent<Animator>();

            // Stop controls
            if (playerController != null) playerController.SetControlEnabled(false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsMoving", false);

            // Dialogue
            if (DialogueSystem.Instance != null)
            {
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Ồ! Tài sản của cậu đã vượt quá 500.000.000đ rồi sao?" },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Cậu thực sự đã thắng lớn ở sòng bạc này. Cậu được phép rời khỏi khu VIP." },
                    new DialogueLine { speakerName = "Minh", text = "Thật tốt quá... Cuối cùng mình đã có thể đi xuống." },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Chúc mừng nhé, đi xuống đi!" }
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

            // Let the player pass down
            Vector2 startPos = playerObj.transform.position;
            Vector2 targetPos = startPos + new Vector2(1.5f, -1.0f);
            
            yield return StartCoroutine(WalkToPosition(playerObj, startPos, targetPos, new Vector2(1.5f, -1.0f).normalized));

            if (playerController != null) playerController.SetControlEnabled(true);
            yield return new WaitForSeconds(0.5f);
            isChecking = false;
        }

        private IEnumerator HandleBrokePassDialogue(GameObject playerObj)
        {
            isChecking = true;

            var playerController = playerObj.GetComponent<PlayerController>();
            var rb = playerObj.GetComponent<Rigidbody2D>();
            var animator = playerObj.GetComponent<Animator>();

            // Stop controls
            if (playerController != null) playerController.SetControlEnabled(false);
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsMoving", false);

            // Dialogue
            if (DialogueSystem.Instance != null)
            {
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Hết sạch tiền rồi à? Trông cậu thảm hại thật đấy." },
                    new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Đúng là đồ vô dụng. Cút xuống tầng dưới đi!" },
                    new DialogueLine { speakerName = "Minh", text = "Mình... trắng tay thật rồi..." }
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

            // Let the player pass down, walk them down automatically to avoid trigger re-entry loop
            Vector2 startPos = playerObj.transform.position;
            Vector2 targetPos = startPos + new Vector2(1.5f, -1.0f);
            
            yield return StartCoroutine(WalkToPosition(playerObj, startPos, targetPos, new Vector2(1.5f, -1.0f).normalized));

            if (playerController != null) playerController.SetControlEnabled(true);
            yield return new WaitForSeconds(0.5f);
            isChecking = false;
        }

        private IEnumerator WalkToPosition(GameObject playerObj, Vector2 start, Vector2 target, Vector2 dir)
        {
            var rb = playerObj.GetComponent<Rigidbody2D>();
            var animator = playerObj.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", dir.x);
                animator.SetFloat("MoveY", dir.y);
                animator.SetFloat("LastMoveX", dir.x);
                animator.SetFloat("LastMoveY", dir.y);
            }

            float duration = 0.6f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Vector2 currentPos = Vector2.Lerp(start, target, elapsed / duration);
                if (rb != null) rb.MovePosition(currentPos);
                else playerObj.transform.position = currentPos;
                yield return null;
            }

            if (rb != null) rb.position = target;
            else playerObj.transform.position = target;

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }
}
