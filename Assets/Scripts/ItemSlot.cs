using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private GameObject selectedPanel;

    [Header("Description")]
    [SerializeField] private TMP_Text descriptionHeading;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image descriptionImage;

    private string itemName;
    private string itemDescription;
    private Sprite itemSprite;

    private InventoryManager inventoryManager;
    private bool isSelected = false;

    public void SetSlot(string itemName, int quantity, Sprite icon, string description, InventoryManager manager)
    {
        this.itemName = itemName;
        this.itemSprite = icon;
        this.itemDescription = description;
        inventoryManager = manager;

        iconImage.sprite = icon;
        quantityText.text = quantity.ToString();
        selectedPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected)
            {
                Deselect();
                inventoryManager.ClearDescription();
            }
            else
            {
                inventoryManager.DeselectAllSlots();
                Select();
                inventoryManager.ShowDescription(itemName, itemDescription, itemSprite);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryManager.DropItem(itemName);
        }
    }


    public void Select()
    {
        isSelected = true;
        selectedPanel.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;
        selectedPanel.SetActive(false);
    }
}
