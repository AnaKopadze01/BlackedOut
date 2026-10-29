
using UnityEngine;

public class MonsterVisualDirection : MonoBehaviour
{
    [Header("References")]
    public Transform monster;

    [Header("Rotation Settings")]
    public float rotationSmoothness = 5f;
    public float movementThreshold = 0.001f;

    private Vector3 lastPosition;
    private Vector2 smoothedDirection = Vector2.down;

    void Start()
    {
        if (monster == null)
            monster = transform.parent;

        if (monster != null)
            lastPosition = monster.position;
    }

    void LateUpdate()
    {
        if (monster == null)
            return;

        Vector2 movement =
            (Vector2)(monster.position - lastPosition);

        if (movement.sqrMagnitude >
            movementThreshold * movementThreshold)
        {
            Vector2 targetDirection = movement.normalized;

            float smoothing = 1f -
                Mathf.Exp(-rotationSmoothness * Time.deltaTime);

            smoothedDirection = Vector2.Lerp(
                smoothedDirection,
                targetDirection,
                smoothing
            ).normalized;
        }

        if (smoothedDirection.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(
                smoothedDirection.y,
                smoothedDirection.x
            ) * Mathf.Rad2Deg + 90f;

            transform.rotation = Quaternion.Euler(
                0f, 0f, angle
            );
        }

        lastPosition = monster.position;
    }
}
