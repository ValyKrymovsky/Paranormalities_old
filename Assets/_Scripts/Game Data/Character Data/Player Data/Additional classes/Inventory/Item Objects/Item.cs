using UnityEngine;

namespace MyCode.GameData
{
    [CreateAssetMenu(fileName = "NewDefault", menuName = "Inventory System")]
    public class Item : ScriptableObject
    {
        public enum ItemType
        {
            Key,
            Health,
            Usable,
            Equipment
        }

        public string objectName;
        public ItemType itemType;

        [TextArea(15, 20)]
        public string description;
    }
}

