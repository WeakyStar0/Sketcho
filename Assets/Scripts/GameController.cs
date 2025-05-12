using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressAmount = 0;
        progressBar.value = 0;
        Gem.OnGemCollected += UpdateProgress;
    }

    void UpdateProgress(int amount)
    {
        progressAmount += amount;
        progressBar.value = progressAmount;
        Debug.Log("Progress: " + progressAmount);

        if (progressAmount >= 100)
        {
            Debug.Log("Level Complete!");
            // Add level completion logic here
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
