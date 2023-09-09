using UnityEngine;
using UnityEngine.UIElements;
using MyCode.GameData.Inventory;
using MyCode.UI.Inventory;

public class InventorySlot : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlIntAttributeDescription slotIndex =
            new UxmlIntAttributeDescription { name = "slot-index", defaultValue = 0 };

        UxmlEnumAttributeDescription<SlotType> slotType = 
            new UxmlEnumAttributeDescription<SlotType> { name = "slot-type", defaultValue = SlotType.Normal };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as InventorySlot;

            ate.SlotIndex = slotIndex.GetValueFromBag(bag, cc);
            ate.SlotType = slotType.GetValueFromBag(bag, cc);
        }
    }

    public int SlotIndex { get; set; }
    public SlotType SlotType { get; set; }

    public InventoryItem item;


    public VisualElement slot;
    public VisualElement slotImage;


    public VisualElement descriptionElement;
    public VisualElement descriptionImage;
    public Label descriptionText;

    public InventorySlot()
    {
        slot = new VisualElement();
        slot.name = "InventorySlot";
        slot.AddToClassList("inventorySlot");
        hierarchy.Add(slot);

        slotImage = new VisualElement();
        slotImage.name = "Image";
        slotImage.AddToClassList("inventorySlotImage");
        slot.Add(slotImage);

        item = InventoryItem.empty;

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    public void SetItemParameters(InventoryItem _slotItem)
    {
        item = _slotItem;
        slotImage.style.backgroundImage = new StyleBackground(_slotItem.Image);
    }

    public void ResetParameters()
    {
        item = InventoryItem.empty;
        slotImage.style.backgroundImage = null;
    }

    public Sprite GetItemImage()
    {
        return slotImage.style.backgroundImage.value.sprite;
    }

    public void SetSlotImage(Sprite _image)
    {
        slotImage.style.backgroundImage = new StyleBackground(_image);
    }

    private void OnPointerDown(PointerDownEvent _event)
    {
        if (_event.button != 0 || item.Equals(InventoryItem.empty)) return;

        InventoryHandler.StartDrag(_event.position, this);
        slotImage.style.backgroundImage = null;
    }
}
