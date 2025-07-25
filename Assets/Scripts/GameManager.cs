using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelDuration = 120f; // 2 minutes per level
    public Slider levelTimerSlider;
    
    [Header("Item References")]
    public List<Item> allItems = new List<Item>();
    public FlashlightController flashlight;
    
    private int currentLevel = 1;
    private float timeRemaining;
    private List<Item> remainingInspectableItems = new List<Item>();
    private List<Item> pickedUpItems = new List<Item>();

    void Start()
    {
        // Initialize all items as inspectable only
        foreach (Item item in allItems)
        {
            item.itemType = ItemType.Inspectable;
            remainingInspectableItems.Add(item);
        }
        
        timeRemaining = levelDuration;
        StartCoroutine(LevelTimer());
    }

    IEnumerator LevelTimer()
    {
        while (true)
        {
            timeRemaining -= Time.deltaTime;
            levelTimerSlider.value = timeRemaining / levelDuration;
            
            if (timeRemaining <= 0)
            {
                AdvanceLevel();
                timeRemaining = levelDuration;
            }
            
            yield return null;
        }
    }

    void AdvanceLevel()
    {
        currentLevel++;
        
        switch (currentLevel)
        {
            case 2:
                // Level 2: Make 4 random items pickable
                MakeRandomItemsPickable(4);
                break;
                
            case 3:
                // Level 3: Make remaining items pickable and 2 random items usable
                MakeRandomItemsPickable(remainingInspectableItems.Count);
                MakeRandomItemsUsable(2);
                
                // Enable flashlight after inspection
                flashlight.enabled = true;
                break;
                
            default:
                // Game complete or other logic
                break;
        }
    }

    void MakeRandomItemsPickable(int count)
    {
        count = Mathf.Min(count, remainingInspectableItems.Count);
        
        for (int i = 0; i < count; i++)
        {
            if (remainingInspectableItems.Count == 0) break;
            
            int randomIndex = Random.Range(0, remainingInspectableItems.Count);
            Item item = remainingInspectableItems[randomIndex];
            
            item.itemType = ItemType.Pickable;
            item.MarkAsPickable();
            
            remainingInspectableItems.RemoveAt(randomIndex);
        }
    }

    void MakeRandomItemsUsable(int count)
    {
        count = Mathf.Min(count, pickedUpItems.Count);
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, pickedUpItems.Count);
            Item item = pickedUpItems[randomIndex];
            
            item.itemType = ItemType.Usable;
            // You might want to add additional logic here for specific usable behaviors
        }
    }

    // Call this method when an item is picked up
    public void OnItemPickedUp(Item item)
    {
        pickedUpItems.Add(item);
    }
}