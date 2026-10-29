
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGlowController : MonoBehaviour
{
    [Header("References")]
    public PlayerFlashlight playerFlashlight;
    public Light2D playerGlow;

    void Awake()
    {
        if (playerFlashlight == null)
            playerFlashlight = GetComponentInParent<PlayerFlashlight>();

        if (playerGlow == null)
            playerGlow = GetComponent<Light2D>();
    }

    void LateUpdate()
    {
        if (playerFlashlight == null || playerGlow == null)
            return;

        // Glow is ON only when flashlight is OFF.
        playerGlow.enabled = !playerFlashlight.IsFlashlightOn;
    }
}
