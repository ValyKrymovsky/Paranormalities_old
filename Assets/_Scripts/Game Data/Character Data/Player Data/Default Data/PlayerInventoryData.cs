using UnityEngine;
using System;
using MyBox;
using System.Linq;


namespace MyCode.GameData
{
    [CreateAssetMenu(fileName = "NewInventoryData", menuName = "DataObjects/Player/Inventory")]
    public class PlayerInventoryData : ScriptableObject
    {
        [Space]
        [Separator("Inventory", true)]
        [Space]

        [Header("Inventory Object")]
        [Space]
        [SerializeField] private Inventory _inventory;

        [Space]
        [Separator("Drop", true)]
        [Space]

        [Header("Item Drop")]
        [Space]
        [SerializeField] private float dropRange;
        

        private GameObject inventoryUI;
        private bool inventoryOpen;

        public event Action<bool> OnInventoryStateChange;


        // Inventory
        public Inventory Inventory { get => _inventory; set => _inventory = value; }

        // Item drop
        public float DropRange { get => dropRange; set => dropRange = value; }

        public GameObject InventoryUI { get => inventoryUI; set => inventoryUI = value; }

        // Inventory state
        public bool InventoryOpen { get => inventoryOpen; set => inventoryOpen = value; }


        public void InvokeOnInventoryStateChange(bool _newState)
        {
            OnInventoryStateChange?.Invoke(_newState);
        }

    }

}
