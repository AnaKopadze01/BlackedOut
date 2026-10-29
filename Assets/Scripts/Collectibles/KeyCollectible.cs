
using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only the player can collect this key.
        if (!other.CompareTag("Player"))
            return;

        // Prevent the same key from being collected twice.
        if (collected)
            return;

        collected = true;

        // Notify the GameManager.
        GameManager gameManager =
            FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.CollectKey();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning(
                "Key could not find GameManager!"
            );

            collected = false;
        }
    }
}
