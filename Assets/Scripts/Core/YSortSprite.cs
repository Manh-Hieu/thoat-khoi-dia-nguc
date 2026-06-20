using UnityEngine;

namespace EscapeFromHell.Core
{
    /// <summary>
    /// Automatically updates SpriteRenderer.sortingOrder based on the object's Y position.
    /// Objects lower on screen (smaller Y) = higher sortingOrder = renders in front.
    /// </summary>
    public class YSortSprite : MonoBehaviour
    {
        [Header("Y-Sort Settings")]
        [SerializeField] private int baseOrder = 0;
        [SerializeField] private float yScale = 100f;
        [SerializeField] private float yOffset = 0f;
        [SerializeField] private float fixedSortY = 0f;
        [SerializeField] private bool useFixedY = false;

        private SpriteRenderer sr;

        private void Awake()
        {
            InitSR();
        }

        private void InitSR()
        {
            if (sr != null) return;
            sr = GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = GetComponentInChildren<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (sr == null) InitSR();
            if (sr == null) return;
            UpdateSortingOrder();
        }

        private void UpdateSortingOrder()
        {
            float sortY = useFixedY ? fixedSortY : (transform.position.y + yOffset);
            sr.sortingOrder = baseOrder + Mathf.RoundToInt(-sortY * yScale);
        }

        /// <summary>Configure from code (safe to call from Editor scripts before Awake).</summary>
        public void Configure(int baseOrder, float yScale = 100f, float yOffset = 0f, bool useFixedY = false, float fixedSortY = 0f)
        {
            this.baseOrder = baseOrder;
            this.yScale = yScale;
            this.yOffset = yOffset;
            this.useFixedY = useFixedY;
            this.fixedSortY = fixedSortY;
            InitSR(); // may succeed at runtime, no-op if sr still null in Editor
            if (sr != null) UpdateSortingOrder();
        }
    }
}
