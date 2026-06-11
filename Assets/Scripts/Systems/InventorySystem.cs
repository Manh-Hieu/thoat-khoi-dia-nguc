using UnityEngine;
using System;
using System.Collections.Generic;

namespace EscapeFromHell.Systems
{
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int maxSlots = 6;
        [SerializeField] private List<ItemData> startingItems = new List<ItemData>();

        private List<ItemData> items = new List<ItemData>();

        public event Action<ItemData> OnItemAdded;
        public event Action<ItemData> OnItemRemoved;
        public event Action OnInventoryChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Populate starting items
            foreach (var item in startingItems)
            {
                AddItem(item);
            }
        }

        public List<ItemData> GetItems()
        {
            return new List<ItemData>(items);
        }

        public bool AddItem(ItemData item)
        {
            if (item == null) return false;
            if (items.Count >= maxSlots)
            {
                Debug.LogWarning("Túi đồ đã đầy!");
                return false;
            }

            items.Add(item);
            OnItemAdded?.Invoke(item);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Đã nhặt vật phẩm: {item.itemName}");
            return true;
        }

        public bool RemoveItem(ItemData item)
        {
            if (item == null) return false;
            if (items.Contains(item))
            {
                items.Remove(item);
                OnItemRemoved?.Invoke(item);
                OnInventoryChanged?.Invoke();
                Debug.Log($"Đã loại bỏ vật phẩm: {item.itemName}");
                return true;
            }
            return false;
        }

        public bool HasItem(string itemId)
        {
            return items.Exists(i => i.id == itemId);
        }

        public bool HasItem(ItemData item)
        {
            return items.Contains(item);
        }

        public int FreeSlotsCount()
        {
            return maxSlots - items.Count;
        }

        public int MaxSlots => maxSlots;
    }
}
