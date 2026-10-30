
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
    [Range(0.05f, 0.5f)]
    public float rotationSmoothTime = 0.15f;

    private float rotationVelocity;

    private float currentEnergy;
    private bool flashlightOn = true;

    // Allows other scripts to check whether the flashlight is on.
    public bool IsFlashlightOn => flashlightOn;

    // Returns the remaining battery as a percentage from 0 to 1.
    public float EnergyPercentage => currentEnergy / maxEnergy;

    void Start()
    {
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
        if (flashlightTransform == null || Camera.main == null)
            return;

        // Convert mouse position from screen space to world space.
        Vector3 mousePosition = Input.mousePosition;

        float cameraDistance = Mathf.Abs(
            flashlightTransform.position.z -
            Camera.main.transform.position.z
        );

        mousePosition.z = cameraDistance;

        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(mousePosition);

        // Direction from the flashlight toward the mouse.
        Vector2 direction =
            (Vector2)(mouseWorldPosition - flashlightTransform.position);

        if (direction.sqrMagnitude < 0.0001f)
            return;

        // The flashlight beam points upward at zero rotation.
        float targetAngle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg - 90f;

        float currentAngle = flashlightTransform.localEulerAngles.z;

        // Smoothly rotate toward the mouse.
        float newAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref rotationVelocity,
            rotationSmoothTime
        );

        // Rotate only the flashlight, not the Player GameObject.
        flashlightTransform.localRotation =
            Quaternion.Euler(0f, 0f, newAngle);
    }
}
