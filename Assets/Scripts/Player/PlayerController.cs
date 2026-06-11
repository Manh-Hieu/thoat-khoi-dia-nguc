using UnityEngine;
using EscapeFromHell.Core;

namespace EscapeFromHell.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 5f;
        
        private Rigidbody2D rb;
        private Animator animator;
        private Vector2 movement;
        private Vector2 lastMoveDirection = Vector2.down;
        private bool isRunning = false;
        private bool isControlEnabled = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            // Setup Rigidbody2D properties for pixel perfect top-down
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.freezeRotation = true;
        }

        private void Update()
        {
            // Reset movement input
            movement = Vector2.zero;

            // Only process input if game is in Playing state and controls are enabled
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            {
                rb.linearVelocity = Vector2.zero;
                UpdateAnimator(Vector2.zero);
                return;
            }

            if (!isControlEnabled)
            {
                rb.linearVelocity = Vector2.zero;
                UpdateAnimator(Vector2.zero);
                return;
            }

            // Input reading
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Normalize input to prevent fast diagonal movement
            if (movement.magnitude > 1f)
            {
                movement = movement.normalized;
            }

            // Running input (Shift or Space depending on configuration, we'll support LeftShift for general running and space for action)
            isRunning = Input.GetKey(KeyCode.LeftShift);

            // Record last direction for idle states
            if (movement.x != 0 || movement.y != 0)
            {
                lastMoveDirection = movement.normalized;
            }

            UpdateAnimator(movement);
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (!isControlEnabled)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            // Move character
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            rb.linearVelocity = movement * currentSpeed;
        }

        private void UpdateAnimator(Vector2 move)
        {
            bool isMoving = move.sqrMagnitude > 0;
            animator.SetFloat("MoveX", move.x);
            animator.SetFloat("MoveY", move.y);
            animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                animator.SetFloat("LastMoveX", move.x);
                animator.SetFloat("LastMoveY", move.y);
            }
            else
            {
                animator.SetFloat("LastMoveX", lastMoveDirection.x);
                animator.SetFloat("LastMoveY", lastMoveDirection.y);
            }
        }

        public void SetControlEnabled(bool enabled)
        {
            isControlEnabled = enabled;
            if (!enabled)
            {
                movement = Vector2.zero;
                if (rb != null) rb.linearVelocity = Vector2.zero;
                UpdateAnimator(Vector2.zero);
            }
        }

        public Vector2 GetLastMoveDirection()
        {
            return lastMoveDirection;
        }
    }
}
