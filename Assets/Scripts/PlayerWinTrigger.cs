using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWinTrigger : MonoBehaviour
{
    [Header("Winning UI")]
    [SerializeField] private GameObject winScreenUI;

    private bool hasTriggered = false;

    private void Start()
    {
        if (winScreenUI != null)
            winScreenUI.SetActive(false); // Ensure hidden at start
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player") && winScreenUI != null)
        {
            hasTriggered = true;
            StartCoroutine(ShowWinScreenAfterDelay());
        }
    }

    private IEnumerator ShowWinScreenAfterDelay()
    {
        yield return new WaitForSeconds(3f); // Wait 2 real-time seconds

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;                 // Pause the game AFTER delay
        winScreenUI.SetActive(true);         // Show the win screen
    }

    // Assign this to the UI button OnClick
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Reset time before switching scenes
        SceneManager.LoadScene("MainMenu"); // Replace with your actual scene name
    }
}
