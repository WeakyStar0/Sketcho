using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("Progress Settings")]
    [SerializeField] private int progressAmount;
    [SerializeField] private Slider progressBar;
    [SerializeField] private int winValue = 5;
    [SerializeField] private string nextSceneName;
    [SerializeField] private int nextSceneBuildIndex = -1; // Alternative loading method

    [Header("Game Over Settings")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text gameOverText;

    [Header("Reset Settings")]
    [Tooltip("Scene to load on reset. Leave empty to reload current scene.")]
    [SerializeField] private string resetSceneName;

    [Header("Pause Settings")]
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;
    public static System.Action<bool> OnPauseStateChanged;

    private void Start()
    {
        ResetGameState();
        Gem.OnGemCollected += UpdateProgress;
        PlayerHealth.OnPlayerDeath += GameOverScreen;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log($"Current scene: {SceneManager.GetActiveScene().name}");
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Next scene name: {nextSceneName}");
        }
        if (nextSceneBuildIndex >= 0)
        {
            Debug.Log($"Next scene build index: {nextSceneBuildIndex}");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverScreen.activeSelf)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void ResetGameState()
    {
        progressAmount = 0;
        progressBar.value = 0;
        Time.timeScale = 1;
        gameOverScreen.SetActive(false);
        isPaused = false;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    public void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        gameOverText.text = "BETTER LUCK\nNEXT TIME";
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        BGmusic.instance?.StopMusic();
    }

    public void ResetGame()
    {
        Time.timeScale = 1;
        Gem.OnGemCollected -= UpdateProgress;
        PlayerHealth.OnPlayerDeath -= GameOverScreen;

        BGmusic.instance?.RestartMusic();

        if (string.IsNullOrEmpty(resetSceneName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(resetSceneName);
        }
    }

    #region Pause Menu Functions
    public void PauseGame()
    {
        if (gameOverScreen.activeSelf) return;

        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnPauseStateChanged?.Invoke(true);

        BGmusic.instance?.LowerVolume(0.3f);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OnPauseStateChanged?.Invoke(false);

        BGmusic.instance?.RestoreVolume();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    private void UpdateProgress(int amount)
    {
        progressAmount += amount;
        progressBar.value = progressAmount;

        Debug.Log($"Gem collected! Progress: {progressAmount}/{winValue}");

        if (progressAmount >= winValue)
        {
            Debug.Log("Win condition reached! Loading next scene...");
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.1f);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        if (nextSceneBuildIndex >= 0 && nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneBuildIndex);
        }
        else
        {
            Debug.LogError("No valid scene to load! Check GameController settings.");
        }
    }

    //go to credits button
    public void GoToCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Credits");
    }


    private void OnDestroy()
    {
        Gem.OnGemCollected -= UpdateProgress;
        PlayerHealth.OnPlayerDeath -= GameOverScreen;
    }
}
