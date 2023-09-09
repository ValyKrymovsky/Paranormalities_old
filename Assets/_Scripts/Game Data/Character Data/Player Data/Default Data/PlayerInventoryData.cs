using UnityEngine;
using System;
using MyBox;
using MyCode.GameData.Inventory;
using System.Linq;


namespace MyCode.GameData.PlayerData
{
    [CreateAssetMenu(fileName = "NewInventoryData", menuName = "DataObjects/Player/Inventory")]
    public class PlayerInventoryData : ScriptableObject
    {
        [Space]
        [Separator("Inventory", true)]
        [Space]

        [Header("Inventory Object")]
        [Space]
        [SerializeField] private InventoryItem[] _inventory;
        [SerializeField] private InventoryItem _primaryEquipment;
        [SerializeField] private InventoryItem _secondaryEquipment;

        [Space]
        [Separator("Drop", true)]
        [Space]

        [Header("Item Drop")]
        [Space]
        [SerializeField] private float dropRange;
        

        private GameObject inventoryUI;
        private bool inventoryOpen;


        // Inventory
        public InventoryItem[] Inventory { get => _inventory; set => _inventory = value; }
        public InventoryItem PrimaryEquipment { get => _primaryEquipment; set => _primaryEquipment = value; }
        public InventoryItem SecondaryEquipment { get => _secondaryEquipment; set => _secondaryEquipment = value; }

        // Item drop
        public float DropRange { get => dropRange; set => dropRange = value; }

        public GameObject InventoryUI { get => inventoryUI; set => inventoryUI = value; }

        // Inventory state
        public bool InventoryOpen { get => inventoryOpen; set => inventoryOpen = value; }
    }

}
