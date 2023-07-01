using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Space]
    [Header("Ghost Icon")]
    private static VisualElement ghostIcon;
    private static bool isDragging;
    private static InventorySlot originalSlot;

    private void Awake()
    {
        inventoryUI = GetComponent<UIDocument>();
        root = inventoryUI.rootVisualElement;

        descriptionElement = root.Q<VisualElement>("ItemDescription");
        descriptionImage = descriptionElement.Q<VisualElement>("Image");
        descriptionText = descriptionElement.Q<Label>("Description");

        inventoryGrid = root.Q<VisualElement>("InventoryGrid");

        ghostIcon = root.Q<VisualElement>("GhostIcon");

        foreach(InventorySlot inventorySlot in inventoryGrid.Children())
        {
            inventorySlots.Add(inventorySlot);
        }

        foreach(InventorySlot inventorySlot in inventorySlots)
        {
            inventorySlot.RegisterCallback<MouseOverEvent>((type) =>
            {
                if (!inventorySlot.item.Equals((null, null)) && inventorySlot.slotImage.style.backgroundImage.value.sprite != null)
                {
                    SetDescription(inventorySlot.item.item.description, inventorySlot.slotImage.style.backgroundImage.value.sprite);
                }
            });

            inventorySlot.RegisterCallback<MouseOutEvent>((type) =>
            {
                ResetDescription();
            });
        }

        ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
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

    public static void StartDrag(Vector2 _position, InventorySlot _originalSlot)
    {
        isDragging = true;
        originalSlot = _originalSlot;

        ghostIcon.style.top = _position.y - ghostIcon.layout.height / 2;
        ghostIcon.style.left = _position.x - ghostIcon.layout.width / 2;

        ghostIcon.style.backgroundImage = new StyleBackground(_originalSlot.GetItemImage());

        ghostIcon.style.visibility = Visibility.Visible;
    }

    private void OnPointerMove(PointerMoveEvent _event)
    {
        if (!isDragging) return;

        ghostIcon.style.top = _event.position.y - ghostIcon.layout.height / 2;
        ghostIcon.style.left = _event.position.x - ghostIcon.layout.width / 2;
    }

    private void OnPointerUp(PointerUpEvent _event)
    {
        if (!isDragging) return;

        IEnumerable<InventorySlot> overlapingSlots = inventorySlots.Where(x => x.worldBound.Overlaps(ghostIcon.worldBound));

        if (overlapingSlots.Count() != 0)
        {
            InventorySlot closestSlot = overlapingSlots.OrderBy(x => Vector2.Distance(x.worldBound.position, ghostIcon.worldBound.position)).First();

            if (!closestSlot.item.Equals((null, null)))
            {
                originalSlot.SetSlotImage(ghostIcon.style.backgroundImage.value.sprite);
                isDragging = false;
                originalSlot = null;
                ghostIcon.style.visibility = Visibility.Hidden;
                return;
            }

            closestSlot.SetItemParameters(originalSlot.item.item, originalSlot.item.model, ghostIcon.style.backgroundImage.value.sprite);

            originalSlot.ResetParameters();
        }
        else
        {
            originalSlot.SetSlotImage(ghostIcon.style.backgroundImage.value.sprite);
        }

        isDragging = false;
        originalSlot = null;
        ghostIcon.style.visibility = Visibility.Hidden;

    }
}
