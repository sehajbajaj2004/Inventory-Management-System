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
    private Queue<string> activeBatteries = new Queue<string>(); // Batteries currently in use
    private float batteryTimer = 0f;

    void Start()
    {
        inventoryManager = GameObject.Find("Inventory Canvas").GetComponent<InventoryManager>();
        UpdateBatteryUI();
    }

    void Update()
    {
        bool hasFlashlight = inventoryManager.HasItem("Flashlight", 1);
        flashlightArms.SetActive(hasFlashlight);

        if (!hasFlashlight)
        {
            if (flashState) ToggleFlashlight(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (activeBatteries.Count > 0)
            {
                flashState = !flashState;
                ToggleFlashlight(flashState);
            }
        }

        if (flashState && activeBatteries.Count > 0)
        {
            batteryTimer += Time.deltaTime;
            float t = 1f - (batteryTimer / batteryDuration);
            flashLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

            if (batteryTimer >= batteryDuration)
            {
                batteryTimer = 0f;
                activeBatteries.Dequeue();
                inventoryManager.ReduceItem("Battery", 1);
                UpdateBatteryUI();
                StartCoroutine(ShowBatteryUsedUI());
                
                if (activeBatteries.Count == 0)
                {
                    ToggleFlashlight(false);
                }
            }
        }
    }

    public void AddBatteryToFlashlight()
    {
        if (inventoryManager.HasItem("Battery", 1))
        {
            activeBatteries.Enqueue("Battery");
            UpdateBatteryUI();
        }
    }

    void ToggleFlashlight(bool state)
    {
        flashLightPointLight.SetActive(state);
        flashState = state;
    }

    void UpdateBatteryUI()
    {
        int count = activeBatteries.Count;
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
}