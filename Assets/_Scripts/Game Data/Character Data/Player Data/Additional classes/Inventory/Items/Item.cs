using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace MyCode.GameData
{
    public enum ItemType
    {
        None,
        Key,
        Health,
        Usable,
        Equipment
    }

    public class Item : ScriptableObject
    {
        public string itemName;
        public ItemType itemType;

        [TextArea(15, 20)]
        public string description;

        public int itemId;
        [JsonIgnore] public GameObject itemModel;
        [JsonIgnore] public Sprite itemIcon;

        public static Item empty = null;



        public static List<Item> allItems = new List<Item>();

        public Item()
        {
            this.itemName = null;
            this.itemType = ItemType.None;
            this.description = null;
            this.itemId = 0;
            this.itemModel = null;
            this.itemIcon = null;

        }

        /// <summary>
        /// Checks if item is registered
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>Whether the item is registered or not</returns>
        public static bool Exists(int itemId)
        {
            if (allItems.Exists(i => i.itemId == itemId)) return true;

            return false;
        }

        /// <summary>
        /// Returns Item with matching id
        /// </summary>
        /// <param name="itemId">Id of the item</param>
        /// <returns>Item</returns>
        public static Item GetItem(int itemId)
        {
            if (Exists(itemId)) return allItems.Where(i => i.itemId == itemId).First();

            return Item.empty;
        }

        /// <summary>
        /// Registers the Item if not already registered
        /// </summary>
        /// <param name="item"></param>
        public static void RegisterItem(Item item)
        {
            if (allItems.Exists(i => i.itemId == item.itemId)) return;

            allItems.Add(item);
        }

        /// <summary>
        /// Returns all registered Items
        /// </summary>
        /// <returns>IEnumerable containing all registered Items</returns>
        public static IEnumerable GetAllItems()
        {
            foreach (Item item in allItems) yield return item;
        }
    }
}

