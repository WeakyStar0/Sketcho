using UnityEngine;

public class OpenURLButton : MonoBehaviour
{
    [Header("Link Settings")]
    [SerializeField] private string url = "https://github.com/WeakyStar0/Sketcho";

    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
