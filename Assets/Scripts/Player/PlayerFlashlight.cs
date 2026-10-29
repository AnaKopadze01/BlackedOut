using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerFlashlight : MonoBehaviour
{
    [Header("Flashlight References")]
    public Light2D flashlight;
    public Transform flashlightTransform;

    [Header("Battery Settings")]
    public float maxEnergy = 100f;
    public float drainRate = 10f;
    public float rechargeRate = 5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 360f;

    private float currentEnergy;
    private bool flashlightOn = true;

    private PlayerMovement playerMovement;

    // Allows other scripts to check whether the flashlight is on.
    public bool IsFlashlightOn => flashlightOn;

    // Returns the remaining battery as a percentage from 0 to 1.
    public float EnergyPercentage => currentEnergy / maxEnergy;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Start with a fully charged flashlight.
        currentEnergy = maxEnergy;
        flashlight.enabled = flashlightOn;
    }

    void Update()
    {
        HandleFlashlightToggle();
        UpdateBattery();
        RotateFlashlight();
    }

    void HandleFlashlightToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Cannot turn on the flashlight if the battery is empty.
            if (!flashlightOn && currentEnergy <= 0f)
                return;

            flashlightOn = !flashlightOn;
            flashlight.enabled = flashlightOn;
        }
    }

    void UpdateBattery()
    {
        if (flashlightOn)
        {
            // Drain battery while the flashlight is on.
            currentEnergy -= drainRate * Time.deltaTime;

            if (currentEnergy <= 0f)
            {
                currentEnergy = 0f;
                flashlightOn = false;
                flashlight.enabled = false;
            }
        }
        else
        {
            // Recharge battery while the flashlight is off.
            currentEnergy += rechargeRate * Time.deltaTime;
        }

        // Keep battery energy between 0 and its maximum.
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
    }

    void RotateFlashlight()
    {
        Vector2 direction = playerMovement.MovementDirection;

        // Keep the previous direction when the player stops.
        if (direction.sqrMagnitude < 0.01f)
            return;

        // Calculate the angle corresponding to movement direction.
        float targetAngle = Mathf.Atan2(direction.y, direction.x)
                            * Mathf.Rad2Deg - 90f;

        // Get the flashlight's current angle.
        float currentAngle = flashlightTransform.localEulerAngles.z;

        // Smoothly rotate toward the target angle.
        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        // Apply the new rotation without rotating the player.
        flashlightTransform.localRotation =
            Quaternion.Euler(0f, 0f, newAngle);
    }
}
