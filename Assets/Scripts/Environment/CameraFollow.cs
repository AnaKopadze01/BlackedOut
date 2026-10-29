using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;
    public float smoothSpeed = 5f;

    private Vector3 offset;

    void Start()
    {
        // Remember the camera's starting offset from the player.
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        // Calculate where the camera should move.
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera toward the player.
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
