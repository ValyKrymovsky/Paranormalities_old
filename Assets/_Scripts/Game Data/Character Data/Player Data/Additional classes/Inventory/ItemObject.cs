using UnityEngine;

namespace MyCode.Player.Inventory
{
    [CreateAssetMenu(fileName = "NewDefault", menuName = "Inventory System")]
    public abstract class ItemObject : ScriptableObject
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

