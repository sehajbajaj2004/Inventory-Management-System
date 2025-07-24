using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight References")]
    public GameObject flashLightPointLight;
    public GameObject flashlightArms;
    public Light flashLight;
    public GameObject batteryUsedUI;
    public Image[] batteryUIIcons;

    [Header("Flashlight Settings")]
    public float maxIntensity = 2f;
    public float minIntensity = 0.5f;
    public float batteryDuration = 60f;

    private bool flashState = false;
    private InventoryManager inventoryManager;
    private Queue<string> batteries = new Queue<string>();
    private float batteryTimer = 0f;
    // private bool isConsumingBattery = false;

    void Start()
    {
        inventoryManager = GameObject.Find("Inventory Canvas").GetComponent<InventoryManager>();
        UpdateBatteryQueue();
        UpdateBatteryUI();
    }

    void Update()
    {
        // Check flashlight availability
        bool hasFlashlight = inventoryManager.HasItem("Flashlight", 1);
        flashlightArms.SetActive(hasFlashlight);

        if (!hasFlashlight)
        {
            if (flashState) ToggleFlashlight(false);
            return;
        }

        if (batteries.Count != inventoryManager.GetItemCount("Battery"))
        {
            UpdateBatteryQueue();
            UpdateBatteryUI();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (batteries.Count > 0)
            {
                flashState = !flashState;
                ToggleFlashlight(flashState);
            }
        }

        if (flashState && batteries.Count > 0)
        {
            batteryTimer += Time.deltaTime;
            float t = 1f - (batteryTimer / batteryDuration);
            flashLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

            if (batteryTimer >= batteryDuration)
            {
                batteryTimer = 0f;
                batteries.Dequeue();
                inventoryManager.ReduceItem("Battery", 1);
                UpdateBatteryUI();
                StartCoroutine(ShowBatteryUsedUI());
            }
        }
    }

    void ToggleFlashlight(bool state)
    {
        flashLightPointLight.SetActive(state);
        flashState = state;
    }

    void UpdateBatteryQueue()
    {
        batteries.Clear();
        int count = inventoryManager.GetItemCount("Battery");
        for (int i = 0; i < count; i++) batteries.Enqueue("Battery");
    }

    // void ToggleFlashlight(bool state)
    // {
    //     flashLightPointLight.SetActive(state);
    //     flashState = state;
    // }

    // void UpdateBatteryQueue()
    // {
    //     batteries.Clear();
    //     int count = inventoryManager.GetItemCount("Battery");
    //     for (int i = 0; i < count; i++) batteries.Enqueue("Battery");
    // }

    void UpdateBatteryUI()
    {
        int count = batteries.Count;
        for (int i = 0; i < batteryUIIcons.Length; i++)
        {
            batteryUIIcons[i].enabled = i < count;
        }
    }

    IEnumerator ShowBatteryUsedUI()
    {
        batteryUsedUI.SetActive(true);
        yield return new WaitForSeconds(5f);
        batteryUsedUI.SetActive(false);
    }
    // IEnumerator ShowBatteryUsedUI()
    // {
    //     batteryUsedUI.SetActive(true);
    //     yield return new WaitForSeconds(5f);
    //     batteryUsedUI.SetActive(false);
    // }
}
