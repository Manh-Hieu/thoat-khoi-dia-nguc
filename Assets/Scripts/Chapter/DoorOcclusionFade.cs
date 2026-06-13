using UnityEngine;

namespace EscapeFromHell.Chapter
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DoorOcclusionFade : MonoBehaviour
    {
        [Header("Fade Settings")]
        [Range(0f, 1f)] public float normalAlpha = 1f;
        [Range(0f, 1f)] public float fadedAlpha = 0.55f;
        public float fadeSpeed = 8f;

        [Header("Detection")]
        public float triggerRadius = 1.5f;
        public Vector2 overlapBoxSize = new Vector2(1.4f, 2.0f); // legacy, unused

        private SpriteRenderer _sr;
        private Transform _playerTransform;
        private float _targetAlpha;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _targetAlpha = normalAlpha;
        }

        private void Start()
        {
            FindPlayer();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) player = GameObject.Find("Player");
            if (player != null)
                _playerTransform = player.transform;
        }

        private void Update()
        {
            if (_playerTransform == null)
            {
                FindPlayer();
                return;
            }

            float dist = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                new Vector2(_playerTransform.position.x, _playerTransform.position.y)
            );

            bool occluded = dist < triggerRadius;
            _targetAlpha = occluded ? fadedAlpha : normalAlpha;

            Color c = _sr.color;
            c.a = Mathf.Lerp(c.a, _targetAlpha, Time.deltaTime * fadeSpeed);
            _sr.color = c;
        }
    }
}
