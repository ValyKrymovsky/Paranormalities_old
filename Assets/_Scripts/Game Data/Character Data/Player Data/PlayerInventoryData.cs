using UnityEngine.InputSystem;
using UnityEngine;
using System;
using MyBox;
using MyCode.Player.Inventory;


namespace MyCode.Data.Player
{
    [CreateAssetMenu(fileName = "NewInventoryData", menuName = "DataObjects/Player/Inventory")]
    public class PlayerInventoryData : ScriptableObject
    {
        [Space]
        [Separator("Inventory", true)]
        [Space]

        [Header("Inventory Object")]
        [Space]
        [SerializeField] private InventoryObject inventory;

        [Space]
        [Separator("Drop", true)]
        [Space]

        [Header("Item Drop")]
        [Space]
        [SerializeField] private float dropRange;

        [Space]
        [Separator("Inputs", true)]
        [Space]

        [Header("Input Action")]
        [Space]
        [SerializeField] private InputActionReference input_DropItem;
        [SerializeField] private InputActionReference input_ToggleInventory;

        private GameObject inventoryUI;
        private bool inventoryOpen;

        public event Action<bool> OnInventoryStatusChange;
        public event Action<ItemObject, GameObject, Sprite> OnAddItem;


        public InventoryObject Inventory { get => inventory; set => inventory = value; }
        public float DropRange { get => dropRange; set => dropRange = value; }
        public InputActionReference DropItemInput { get => input_DropItem; }
        public InputActionReference ToggleInventoryInput { get => input_ToggleInventory; }
        public GameObject InventoryUI { get => inventoryUI; set => inventoryUI = value; }
        public bool InventoryOpen { get => inventoryOpen; set => inventoryOpen = value; }


        public void InvokeOnInventoryStatusChange(bool _newState)
        {
            OnInventoryStatusChange?.Invoke(_newState);
        }

        public void InvokeOnAddItem(ItemObject _item, GameObject _model, Sprite _image)
        {
            Debug.Log("add item action invoked");
            OnAddItem?.Invoke(_item, _model, _image);
        }

        private void OnEnable()
        {
            input_DropItem.action.Enable();
            input_ToggleInventory.action.Enable();
        }

        private void OnDisable()
        {
            input_DropItem.action.Disable();
            input_ToggleInventory.action.Disable();
        }
    }

}
