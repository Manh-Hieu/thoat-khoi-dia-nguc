using UnityEngine;

namespace EscapeFromHell.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Settings")]
        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            if (target == null)
            {
                // Try to find player automatically
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
