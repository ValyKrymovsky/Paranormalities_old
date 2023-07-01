using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using MyBox;

public class InventorySlot : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> {}

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlIntAttributeDescription slotIndex = 
            new UxmlIntAttributeDescription { name = "slot-index", defaultValue = 0};

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as InventorySlot;

            ate.SlotIndex = slotIndex.GetValueFromBag(bag, cc);
        }
    }

    public int SlotIndex { get; set; }

    public InventoryObject inventory;
    public (ItemObject item, GameObject model) item = (null, null);

    [Separator("Slot Element", true)]
    public VisualElement slot;
    public Button slotCollider;
    public VisualElement slotImage;

    [Separator("Description Element", true)]
    public VisualElement descriptionElement;
    public VisualElement descriptionImage;
    public Label descriptionText;

    public InventorySlot()
    {
        slot = new VisualElement();
        slot.name = "InventorySlot";
        slot.AddToClassList("inventorySlot");
        hierarchy.Add(slot);

        slotCollider = new Button();
        slotCollider.focusable = false;
        slotCollider.name = "SlotCollision";
        slotCollider.AddToClassList("inventorySlotCollider");
        slot.Add(slotCollider);

        slotImage = new VisualElement();
        slotImage.name = "Image";
        slotImage.AddToClassList("inventorySlotImage");
        slotCollider.Add(slotImage);

        RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
    }

    public void SetItemParameters(ItemObject _item, GameObject _model, Sprite _itemImage)
    {
        item = (_item, _model);
        slotImage.style.backgroundImage = new StyleBackground(_itemImage);
    }

    public void ResetParameters()
    {
        item = (null, null);
        slotImage.style.backgroundImage = null;
    }

    public Button GetSlotButton()
    {
        return slotCollider;
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
        if (_event.button != 0 || item.Equals((null, null))) return;

        InventoryEventHandler.StartDrag(_event.position, this);
        slotImage.style.backgroundImage = null;
    }
}