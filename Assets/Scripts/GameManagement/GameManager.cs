
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    private static bool restartImmediately = false;

    [Header("Game UI")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Start Menu")]
    public GameObject startPanel;
    private bool gameStarted = false;

    [Header("Key Counter UI")]
    public TMP_Text keyCounterText;
    [Header("Key Pickup Sound")]
    public AudioClip keyPickupSound;
    public AudioSource keyPickupAudioSource;

    [Header("Exit Message")]
    public GameObject findExitText;

    [Header("Player References")]
    public PlayerMovement playerMovement;

    [Header("Monster References")]
    public MonsterAI monsterAI;
    public MonsterSounds monsterSounds;

    [Header("Victory Sound")]
    public AudioClip victorySound;
    public AudioSource victoryAudioSource;

    [Header("Victory Screen Settings")]
    public float victoryPanelDelay = 0.5f;

    [Header("Key Collection")]
    public int totalKeys = 3;

    [Header("Exit Spawning")]
    public ExitSpawner exitSpawner;

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

        // Hide the victory screen when a new game begins.
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        UpdateKeyCounter();

        if (restartImmediately)
{
    restartImmediately = false;
    gameStarted = true;

    if (startPanel != null)
        startPanel.SetActive(false);

    Time.timeScale = 1f;
}
else
{
    gameStarted = false;

    if (startPanel != null)
        startPanel.SetActive(true);

    Time.timeScale = 0f;
}

if (findExitText != null)
{
    findExitText.SetActive(false);
}
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

        if (HasAllKeys && findExitText != null)
        {
            findExitText.SetActive(true);
        }

        if (HasAllKeys && exitSpawner != null)
        {
            exitSpawner.SpawnExit();
        }

        if (keyPickupAudioSource != null && keyPickupSound != null)
        {
            keyPickupAudioSource.PlayOneShot(keyPickupSound);
        }

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

        // NEW: Play monster attack sound.
        if (monsterSounds != null)
            monsterSounds.PlayAttackSound();

        // Stop player movement.
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Stop the player's walking sound immediately.
        if (playerMovement != null)
        {
            PlayerFootsteps footsteps =
            playerMovement.GetComponent<PlayerFootsteps>();

        if (footsteps != null)
            footsteps.enabled = false;
        }

        // Stop monster movement.
        if (monsterAI != null)
            monsterAI.enabled = false;

        // Show Game Over screen.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Pause gameplay.
        Time.timeScale = 0f;
    }

    public void RestartGame()
{
    restartImmediately = true;
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        // Play the portal escape sound.
        if (victoryAudioSource != null && victorySound != null)
        {
            victoryAudioSource.PlayOneShot(victorySound);
        }

        Debug.Log("You escaped the hospital! You win!");

        // Stop player and monster movement.
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Stop the player's walking sound immediately.
        if (playerMovement != null)
        {
            PlayerFootsteps footsteps =
            playerMovement.GetComponent<PlayerFootsteps>();

        if (footsteps != null)
            footsteps.enabled = false;
        }

        if (monsterAI != null)
            monsterAI.enabled = false;

       // Pause gameplay immediately.
Time.timeScale = 0f;

// Show the victory panel after a short delay.
StartCoroutine(ShowVictoryPanelAfterDelay());
    }

    private System.Collections.IEnumerator ShowVictoryPanelAfterDelay()
{
    yield return new WaitForSecondsRealtime(victoryPanelDelay);

    if (victoryPanel != null)
        victoryPanel.SetActive(true);
}

public void StartGame()
{
    if (gameStarted) return;

    gameStarted = true;

    if (startPanel != null)
        startPanel.SetActive(false);

    Time.timeScale = 1f;
}

public void ReturnToMainMenu()
{
    restartImmediately = false;
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
}
