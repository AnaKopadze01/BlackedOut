
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    // Allows other scripts to read the player's movement direction.
    public Vector2 MovementDirection => movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Get the direction from the player to the mouse.
        Vector3 mousePosition = Input.mousePosition;

        float cameraDistance = Mathf.Abs(
            transform.position.z - Camera.main.transform.position.z
        );

        mousePosition.z = cameraDistance;

        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 forward = (Vector2)(
            mouseWorldPosition - transform.position
        );

        // Avoid undefined direction when the mouse is on the player.
        if (forward.sqrMagnitude < 0.0001f)
            return;

        forward.Normalize();

        // Right direction is perpendicular to forward.
        Vector2 right = new Vector2(forward.y, -forward.x);

        // W/S move forward/backward.
        // A/D strafe left/right.
        movement = (forward * moveY + right * moveX).normalized;
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb.position
            + movement * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }
}
