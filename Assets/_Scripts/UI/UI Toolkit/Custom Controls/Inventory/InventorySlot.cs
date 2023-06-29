using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class InventorySlot : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<InventorySlot> {}

    public InventorySlot()
    {
        VisualElement slot = new VisualElement();
        slot.name = "inventorySlot";
        hierarchy.Add(slot);

        VisualElement slotImage = new VisualElement();
        slotImage.name = "image";
        slot.hierarchy.Add(slotImage);
        
    }
}
