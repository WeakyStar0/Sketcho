using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Progress Settings")]
    [SerializeField] private int progressAmount;
    [SerializeField] private Slider progressBar;

    [Header("Game Over Settings")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text gameOverText;
    
    [Header("Reset Settings")]
    [Tooltip("Scene to load on reset. Leave empty to reload current scene.")]
    [SerializeField] private string resetSceneName;

    private void Start()
    {
        ResetGameState(); // Initialize fresh on start
        Gem.OnGemCollected += UpdateProgress;
        PlayerHealth.OnPlayerDeath += GameOverScreen;
    }

    // Reset ALL game state variables
    private void ResetGameState()
    {
        progressAmount = 0;
        progressBar.value = 0;
        Time.timeScale = 1; // Ensure game is unpaused
        gameOverScreen.SetActive(false);
    }

    public void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        gameOverText.text = "BETTER LUCK\nNEXT TIME";
        Time.timeScale = 0; // Freeze game
    }

    // Called from UI button or death event
    public void ResetGame()
    {
        // Clean up before reset (optional)
        Gem.OnGemCollected -= UpdateProgress;
        PlayerHealth.OnPlayerDeath -= GameOverScreen;

        // Force a full scene reload
        if (string.IsNullOrEmpty(resetSceneName))
        {
            // Reload current scene (hard reset)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
        else
        {
            // Load specified scene (hard reset)
            SceneManager.LoadScene(resetSceneName, LoadSceneMode.Single);
        }
    }

    private void UpdateProgress(int amount)
    {
        progressAmount += amount;
        progressBar.value = progressAmount;
        
        if (progressAmount >= 100)
        {
            Debug.Log("Level Complete!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        Gem.OnGemCollected -= UpdateProgress;
        PlayerHealth.OnPlayerDeath -= GameOverScreen;
    }
}