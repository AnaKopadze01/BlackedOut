
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react when the player enters the exit.
        if (!other.CompareTag("Player"))
            return;

        if (gameManager == null || gameManager.GameEnded)
            return;

        // The player needs all three keys to escape.
        if (gameManager.HasAllKeys)
        {
            gameManager.WinGame();
        }
        else
        {
            Debug.Log(
                "The exit is locked! Collect all three keys."
            );
        }
    }
}
