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

        // Prevent diagonal movement from being faster.
        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb.position
            + movement * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }
}
