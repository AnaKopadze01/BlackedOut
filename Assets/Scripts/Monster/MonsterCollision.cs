
using UnityEngine;

public class MonsterCollision : MonoBehaviour
{
    [Header("Game References")]
    public GameManager gameManager;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check whether the monster collided with the player.
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gameManager != null)
                gameManager.GameOver();
        }
    }
}
