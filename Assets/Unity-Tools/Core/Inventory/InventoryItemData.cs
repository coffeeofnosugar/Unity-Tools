using UnityEngine;

namespace Tools.Inventory
{
    [CreateAssetMenu(menuName = "Inventory Item Data")]
    public class InventoryItemData : ScriptableObject
    {
        public int id;
        public string itemName;
        public Sprite icon;
        public GameObject prefab;
    }
}