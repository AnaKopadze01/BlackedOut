
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game UI")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    [Header("Key Counter UI")]
    public TMP_Text keyCounterText;

    [Header("Player References")]
    public PlayerMovement playerMovement;

    [Header("Monster References")]
    public MonsterAI monsterAI;

    [Header("Key Collection")]
    public int totalKeys = 3;

    private int collectedKeys = 0;
    private bool gameEnded = false;

    public bool GameEnded => gameEnded;
    public int CollectedKeys => collectedKeys;

    public bool HasAllKeys => collectedKeys >= totalKeys;

    void Start()
    {
        // Hide Game Over when a new game begins.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        if (victoryPanel != null)
        victoryPanel.SetActive(false);

        UpdateKeyCounter();
    }

    public void CollectKey()
    {
        // Don't collect anything after the game ends.
        if (gameEnded)
            return;

        // Prevent collecting more than the required number.
        if (collectedKeys >= totalKeys)
            return;

        collectedKeys++;
        UpdateKeyCounter();

        Debug.Log(
            "Key collected! " +
            collectedKeys + "/" + totalKeys
        );

        if (HasAllKeys)
        {
            Debug.Log(
                "All keys collected! Find the exit!"
            );
        }
    }

    public void GameOver()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        Debug.Log(
            "Game Over! The monster caught the player."
        );

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (monsterAI != null)
            monsterAI.enabled = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }


    private void UpdateKeyCounter()
    {
        // Update the visible key collection progress.
        if (keyCounterText != null)
        {
            keyCounterText.text =
                "KEYS: " + collectedKeys + "/" + totalKeys;
        }
    }


public void WinGame()
{
    // Prevent winning more than once.
    if (gameEnded)
        return;

    // Require all keys before allowing victory.
    if (!HasAllKeys)
        return;

    gameEnded = true;

    Debug.Log("You escaped the mansion! You win!");

    // Stop player and monster movement.
    if (playerMovement != null)
        playerMovement.enabled = false;

    if (monsterAI != null)
        monsterAI.enabled = false;

    // Show the victory screen.
    if (victoryPanel != null)
        victoryPanel.SetActive(true);

    // Pause gameplay.
    Time.timeScale = 0f;
}

}
