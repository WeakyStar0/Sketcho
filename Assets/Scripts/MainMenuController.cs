using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField] private string levelToLoad = "GameScene";
    public void OnStartClick()
    {
        // Load the game scene
        SceneManager.LoadScene(levelToLoad);
    }

    public void OnExitClick()
    {
        // If running in the editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        // Quit the application
        Application.Quit();
    }

    //funtion to load credits scene
    public void OnCreditsClick()
    {
        // Load the credits scene
        SceneManager.LoadScene("Credits");
    }
}
