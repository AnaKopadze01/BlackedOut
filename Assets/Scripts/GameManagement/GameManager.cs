
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
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

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

            // Make sure both monsters have their sounds enabled.
            SetMonsterSoundsEnabled(true);
        }
        else
        {
            gameStarted = false;

            if (startPanel != null)
                startPanel.SetActive(true);

            Time.timeScale = 0f;

            // Silence both monsters while the Start Menu is open.
            SetMonsterSoundsEnabled(false);
        }

        if (findExitText != null)
            findExitText.SetActive(false);
    }

    public void CollectKey()
    {
        if (gameEnded)
            return;

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
            Debug.Log("All keys collected! Find the exit!");
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

        // Stop player movement.
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Stop footsteps.
        if (playerMovement != null)
        {
            PlayerFootsteps footsteps =
                playerMovement.GetComponent<PlayerFootsteps>();

            if (footsteps != null)
                footsteps.enabled = false;
        }

        // Stop both monsters and their background sounds.
        // Keep one final attack roar.
        StopAllMonsters(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // Enable or disable background growling for both monsters.
    private void SetMonsterSoundsEnabled(bool enabled)
    {
        MonsterSounds[] allMonsterSounds =
            FindObjectsByType<MonsterSounds>(
                FindObjectsSortMode.None
            );

        foreach (MonsterSounds sounds in allMonsterSounds)
        {
            AudioSource source = sounds.GetComponent<AudioSource>();

            if (!enabled && source != null)
            {
                // Stop any growling already playing.
                source.Stop();
            }

            // Disabled scripts cannot restart the growling.
            sounds.enabled = enabled;
        }
    }

    // Stop all monsters when the game ends.
    private void StopAllMonsters(bool playAttackSound)
    {
        MonsterAI[] monsters =
            FindObjectsByType<MonsterAI>(
                FindObjectsSortMode.None
            );

        foreach (MonsterAI monster in monsters)
        {
            monster.enabled = false;
        }

        MonsterSounds[] allMonsterSounds =
            FindObjectsByType<MonsterSounds>(
                FindObjectsSortMode.None
            );

        foreach (MonsterSounds sounds in allMonsterSounds)
        {
            AudioSource source = sounds.GetComponent<AudioSource>();

            if (source != null)
                source.Stop();

            sounds.enabled = false;
        }

        // Keep the final Game Over attack sound.
        if (playAttackSound && monsterSounds != null)
        {
            monsterSounds.PlayAttackSound();
        }
    }

    public void RestartGame()
    {
        restartImmediately = true;

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
        if (keyCounterText != null)
        {
            keyCounterText.text =
                "KEYS: " + collectedKeys + "/" + totalKeys;
        }
    }

    public void WinGame()
    {
        if (gameEnded)
            return;

        if (!HasAllKeys)
            return;

        gameEnded = true;

        // Play the portal escape sound.
        if (victoryAudioSource != null && victorySound != null)
        {
            victoryAudioSource.PlayOneShot(victorySound);
        }

        Debug.Log("You escaped the hospital! You win!");

        // Stop player movement.
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Stop footsteps.
        if (playerMovement != null)
        {
            PlayerFootsteps footsteps =
                playerMovement.GetComponent<PlayerFootsteps>();

            if (footsteps != null)
                footsteps.enabled = false;
        }

        // Silence both monsters.
        StopAllMonsters(false);

        Time.timeScale = 0f;

        // Show Victory Panel after the existing delay.
        StartCoroutine(ShowVictoryPanelAfterDelay());
    }

    private System.Collections.IEnumerator ShowVictoryPanelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(
            victoryPanelDelay
        );

        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void StartGame()
    {
        if (gameStarted)
            return;

        gameStarted = true;

        if (startPanel != null)
            startPanel.SetActive(false);

        Time.timeScale = 1f;

        // Enable both monsters' background growling.
        SetMonsterSoundsEnabled(true);
    }

    public void ReturnToMainMenu()
    {
        restartImmediately = false;

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
