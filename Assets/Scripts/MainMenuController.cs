using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        // Load the game scene
        SceneManager.LoadScene("Level1");
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
}
