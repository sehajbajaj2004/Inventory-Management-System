using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryMenu;
    public GameObject itemSlotPrefab;
    public Transform itemSlotParent;

    [Header("Description Area")]
    public TMP_Text descriptionHeading;
    public TMP_Text descriptionText;
    public Image descriptionImage;

    [Header("World Drop")]
    public Transform dropTransform;

    private bool menuActivated = false;
    private Dictionary<string, InventoryItemData> itemDictionary = new Dictionary<string, InventoryItemData>();
    private List<ItemSlot> currentSlots = new List<ItemSlot>();

    // Track the selected item
    private string selectedItemName = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            menuActivated = !menuActivated;
            inventoryMenu.SetActive(menuActivated);
            Cursor.visible = menuActivated;
            Cursor.lockState = menuActivated ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void AddItem(string itemName, int quantity, Sprite sprite, string description = "", GameObject dropPrefab = null, UsableType usableType = UsableType.None)
    {
        if (itemDictionary.ContainsKey(itemName))
        {
            itemDictionary[itemName].quantity += quantity;
        }
        else
        {
            InventoryItemData newItem = new InventoryItemData
            {
                itemName = itemName,
                quantity = quantity,
                icon = sprite,
                description = description,
                dropPrefab = dropPrefab,
                usableType = usableType
            };
            itemDictionary.Add(itemName, newItem);
        }

        RefreshInventoryUI();
    }

    public void DropItem(string itemName)
    {
        if (!itemDictionary.ContainsKey(itemName)) return;

        GameObject dropPrefab = itemDictionary[itemName].dropPrefab;

        if (dropPrefab == null)
        {
            Debug.LogWarning($"Drop prefab not assigned for: {itemName}");
            return;
        }

        GameObject dropped = Instantiate(dropPrefab, dropTransform.position, Quaternion.identity);

        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomForce = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(2f, 5f),
                Random.Range(-2f, 2f)
            );
            rb.AddForce(randomForce, ForceMode.Impulse);
        }

        ReduceItem(itemName, 1);
    }

    private void RefreshInventoryUI()
    {
        foreach (Transform child in itemSlotParent)
        {
            Destroy(child.gameObject);
        }

        currentSlots.Clear();

        foreach (var item in itemDictionary.Values)
        {
            GameObject slotGO = Instantiate(itemSlotPrefab, itemSlotParent);
            ItemSlot slot = slotGO.GetComponent<ItemSlot>();
            slot.SetSlot(item.itemName, item.quantity, item.icon, item.description, this);
            currentSlots.Add(slot);
        }

        ClearDescription();
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in currentSlots)
        {
            slot.Deselect();
        }
    }

    public void ShowDescription(string heading, string desc, Sprite icon)
    {
        descriptionHeading.text = heading;
        descriptionText.text = desc;
        descriptionImage.sprite = icon;
        descriptionImage.enabled = true;

        selectedItemName = heading; // Store selected item
    }

    public void ClearDescription()
    {
        descriptionHeading.text = "";
        descriptionText.text = "";
        descriptionImage.sprite = null;
        descriptionImage.enabled = false;

        selectedItemName = null;
    }

    public bool HasItem(string itemName, int minQuantity = 1)
    {
        return itemDictionary.ContainsKey(itemName) && itemDictionary[itemName].quantity >= minQuantity;
    }

    public int GetItemCount(string itemName)
    {
        return itemDictionary.ContainsKey(itemName) ? itemDictionary[itemName].quantity : 0;
    }

    public void ReduceItem(string itemName, int amount)
    {
        if (itemDictionary.ContainsKey(itemName))
        {
            itemDictionary[itemName].quantity -= amount;
            if (itemDictionary[itemName].quantity <= 0)
            {
                itemDictionary.Remove(itemName);
            }
            RefreshInventoryUI();
        }
    }

    // === ✅ USE BUTTON SUPPORT ===
    public void UseSelectedItem()
    {
        if (string.IsNullOrEmpty(selectedItemName)) return;

        if (itemDictionary.TryGetValue(selectedItemName, out InventoryItemData item))
        {
            if (item.usableType == UsableType.Consumable)
            {
                Debug.Log($"Used {selectedItemName}");
                ReduceItem(selectedItemName, 1);
                // Add your item effect logic here (healing, buff, etc.)
            }
            else
            {
                Debug.Log($"{selectedItemName} is not consumable.");
            }
        }
    }

    private class InventoryItemData
    {
        public string itemName;
        public int quantity;
        public Sprite icon;
        public string description;
        public GameObject dropPrefab;
        public UsableType usableType;
    }
}
