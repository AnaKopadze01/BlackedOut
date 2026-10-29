using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Flashlight UI")]
    public Slider flashlightBatteryBar;
    public PlayerFlashlight playerFlashlight;

    void Start()
    {
        // The battery bar displays values between 0 and 1.
        flashlightBatteryBar.minValue = 0f;
        flashlightBatteryBar.maxValue = 1f;
    }

    void Update()
    {
        // Update the bar to match the flashlight's battery.
        flashlightBatteryBar.value =
            playerFlashlight.EnergyPercentage;
    }
}
