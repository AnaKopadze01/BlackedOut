
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Animation Settings")]
    public float animationSpeed = 1f;

    private string currentAnimation = "";

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Start standing still, facing down.
        currentAnimation = "PlayerWalkDown";
        animator.Play(currentAnimation, 0, 0f);
        animator.Update(0f);
        animator.speed = 0f;
    }

    void Update()
    {
        if (animator == null)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // No movement input: stop animating.
        if (Mathf.Abs(horizontal) < 0.01f &&
            Mathf.Abs(vertical) < 0.01f)
        {
            animator.speed = 0f;
            return;
        }

        string nextAnimation;

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            nextAnimation = horizontal > 0f
                ? "PlayerWalkRight"
                : "PlayerWalkLeft";
        }
        else
        {
            nextAnimation = vertical > 0f
                ? "PlayerWalkUp"
                : "PlayerWalkDown";
        }

        if (currentAnimation != nextAnimation)
        {
            animator.Play(nextAnimation, 0, 0f);
            currentAnimation = nextAnimation;
        }

        // Continue advancing walking frames while moving.
        animator.speed = animationSpeed;
    }
}
