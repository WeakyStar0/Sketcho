using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMainMenu : MonoBehaviour
{
    // This function should be linked to your button's OnClick event
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
