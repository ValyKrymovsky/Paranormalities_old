using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

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

    [Serializable]
    public class Item : ScriptableObject
    {
        public string itemName;
        public ItemType type;

        [TextArea(15, 20)]
        public string description;

        public int id;
        [JsonIgnore] public Sprite sprite;
        [JsonIgnore] public static Item empty = null;



        private static List<Item> allItems = new List<Item>();

        public Item()
        {
            this.itemName = null;
            this.type = ItemType.None;
            this.description = null;
            this.id = 0;
            this.sprite = null;

        }

        /// <summary>
        /// Checks if item is registered
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Whether the item is registered or not</returns>
        public static bool Exists(int id)
        {
            if (allItems.Exists(i => i.id == id)) return true;

            return false;
        }

        /// <summary>
        /// Returns Item with matching id
        /// </summary>
        /// <param name="id">Id of the item</param>
        /// <returns>Item</returns>
        public static Item GetItem(int id)
        {
            if (Exists(id)) return allItems.Where(i => i.id == id).First();

            return Item.empty;
        }

        /// <summary>
        /// Registers the Item if not already registered
        /// </summary>
        /// <param name="item"></param>
        public static void RegisterItem(Item item)
        {
            if (allItems.Exists(i => i.id == item.id)) return;

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

