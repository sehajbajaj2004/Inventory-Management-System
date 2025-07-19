using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    [Header("UI Prompts")]
    public GameObject pickupIns;   // "Press E to Pick Up"
    public GameObject inspectIns;  // "Press F to Inspect"

    public void UpdatePrompts(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Inspectable:
                pickupIns?.SetActive(false);
                inspectIns?.SetActive(true);
                break;

            case ItemType.Pickable:
                pickupIns?.SetActive(true);
                inspectIns?.SetActive(true);
                break;

            case ItemType.Usable:
            case ItemType.None:
            default:
                HidePrompts();
                break;
        }
    }

    public void HidePrompts()
    {
        pickupIns?.SetActive(false);
        inspectIns?.SetActive(false);
    }
}
