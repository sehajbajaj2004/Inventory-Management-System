using UnityEngine;

public enum ItemType
{
    None,
    Inspectable,
    Pickable,
    Usable
}

public enum UsableType
{
    None,
    Consumable,
    Persistent
}

public class Item : MonoBehaviour
{
    public string itemName;
    [SerializeField] private int quantity = 1;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] [TextArea] private string itemDescription;

    [Header("Item Type")]
    public ItemType itemType = ItemType.Inspectable;
    public UsableType usableType = UsableType.None;

    [Header("Drop Prefab")]
    public GameObject dropPrefab;

    private bool playerInRange = false;
    private InventoryManager inventoryManager;
    private InstructionManager instructionManager;

    private static bool batteryInspected = false;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        instructionManager = FindObjectOfType<InstructionManager>();

        if (dropPrefab == null) dropPrefab = gameObject;

        instructionManager?.HidePrompts();

        // If this is a battery and one has already been inspected, make it pickable
        if (itemName == "Battery" && batteryInspected)
        {
            itemType = ItemType.Pickable;
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E) && itemType == ItemType.Pickable)
        {
            FindObjectOfType<InspectManager>()?.ForceStopInspecting();

            inventoryManager.AddItem(itemName, quantity, itemSprite, itemDescription, dropPrefab, 
            itemName == "Battery" ? UsableType.Consumable : usableType);

            // If this is a battery, mark that one has been inspected
            if (itemName == "Battery")
            {
                batteryInspected = true;
                MakeAllBatteriesPickable();
            }
            FindObjectOfType<GameManager>()?.OnItemPickedUp(this);

            instructionManager?.HidePrompts();
            Destroy(gameObject);
        }
    }

    private void MakeAllBatteriesPickable()
    {
        Item[] allBatteries = FindObjectsOfType<Item>();
        foreach (Item item in allBatteries)
        {
            if (item.itemName == "Battery" && item.itemType == ItemType.Inspectable)
            {
                item.itemType = ItemType.Pickable;
                item.instructionManager?.UpdatePrompts(ItemType.Pickable);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ray"))
        {
            playerInRange = true;
            instructionManager?.UpdatePrompts(itemType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ray"))
        {
            playerInRange = false;
            instructionManager?.HidePrompts();
        }
    }

    public void MarkAsPickable()
    {
        if (itemType == ItemType.Inspectable)
        {
            itemType = ItemType.Pickable;
            instructionManager?.UpdatePrompts(itemType);
        }
    }
}