using UnityEngine;
using UnityEngine.UIElements;
using MyCode.GameData;
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

    public Item item;

    public VisualElement slotImage;


    public VisualElement descriptionElement;
    public VisualElement descriptionImage;
    public Label descriptionText;

    public InventorySlot()
    {
        slotImage = new VisualElement();
        slotImage.name = "Image";
        slotImage.AddToClassList("inventorySlotImage");
        hierarchy.Add(slotImage);

        item = Item.empty;

        slotImage.RegisterCallback<ClickEvent>(SelectItem);
    }

    public void SetItemParameters(Item _slotItem)
    {
        item = _slotItem;
        slotImage.style.backgroundImage = new StyleBackground(_slotItem.sprite);
    }

    public void ResetParameters()
    {
        item = Item.empty;
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

    private void SelectItem(ClickEvent e)
    {
        if (item != Item.empty)
            Debug.Log("Selected item");
    }
}
