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
    [SerializeField] private string itemName;
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

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        instructionManager = FindObjectOfType<InstructionManager>();

        if (dropPrefab == null) dropPrefab = gameObject;

        instructionManager?.HidePrompts();
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

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E) && itemType == ItemType.Pickable)
        {
            FindObjectOfType<InspectManager>()?.ForceStopInspecting();

            inventoryManager.AddItem(itemName, quantity, itemSprite, itemDescription, dropPrefab, usableType);

            itemType = ItemType.Usable;
            instructionManager?.HidePrompts();

            if (dropPrefab == null) dropPrefab = gameObject;
            Destroy(gameObject);
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