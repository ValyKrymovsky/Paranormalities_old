using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using MyBox;

public class InventoryEventHandler : MonoBehaviour {

    [Separator("Inventory UI", true)]
    [SerializeField] private UIDocument inventoryUI;
    public VisualElement root;
    public VisualElement descriptionElement;
    public VisualElement descriptionImage;
    public Label descriptionText;

    [Space]
    [Header("Inventory Slots")]
    private VisualElement inventoryGrid;
    private List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private void Awake()
    {
        inventoryUI = GetComponent<UIDocument>();
        root = inventoryUI.rootVisualElement;

        descriptionElement = root.Q<VisualElement>("itemDescription");
        descriptionImage = descriptionElement.Q<VisualElement>("image");
        descriptionText = descriptionElement.Q<Label>("description");

        inventoryGrid = root.Q<VisualElement>("inventoryGrid");

        foreach(InventorySlot inventorySlot in inventoryGrid.Children())
        {
            inventorySlots.Add(inventorySlot);
        }

        foreach(InventorySlot inventorySlot in inventorySlots)
        {
            inventorySlot.GetSlotButton().RegisterCallback<MouseOverEvent>((type) =>
            {
                if (!inventorySlot.item.Equals((null, null)) && inventorySlot.slotImage.style.backgroundImage.value.sprite != null)
                {
                    SetDescription(inventorySlot.item.item.description, inventorySlot.slotImage.style.backgroundImage.value.sprite);
                }
            });

            inventorySlot.GetSlotButton().RegisterCallback<MouseOutEvent>((type) =>
            {
                ResetDescription();
            });
            }
    }

    public void SetDescription(string _description, Sprite _itemImage)
    {
        descriptionImage.style.backgroundImage = new StyleBackground(_itemImage);
        descriptionText.text = _description;
    }

    public void ResetDescription()
    {
        descriptionImage.style.backgroundImage = null;
        descriptionText.text = null;
    }
}
