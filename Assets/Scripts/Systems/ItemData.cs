using UnityEngine;

namespace EscapeFromHell.Systems
{
    public enum ItemType
    {
        Key,
        Tool,
        Document,
        Phone
    }

    [CreateAssetMenu(fileName = "NewItemData", menuName = "Escape From Hell/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Item Basic Info")]
        public string id;
        public string itemName;
        [TextArea(3, 5)] public string description;
        public Sprite icon;
        public ItemType itemType;

        [Header("Usage Settings")]
        public bool isUsable = false;
        public string useLocationScene = "";
        public string useTriggerID = "";
    }
}
