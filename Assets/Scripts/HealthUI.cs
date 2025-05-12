using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image heartPrefab; // Reference to the health bar image
    public Sprite fullHeartSprite; // Sprite for a full heart
    public Sprite emptyHeartSprite; // Sprite for an empty heart

    private List<Image> hearts = new List<Image>(); // List to hold the heart images

    public void SetMaxHearts(int maxHearts)
    {
        // Clear existing hearts
        foreach (var heart in hearts)
        {
            Destroy(heart.gameObject);
        }
        hearts.Clear();

        // Create new hearts
        for (int i = 0; i < maxHearts; i++)
        {
            Image heart = Instantiate(heartPrefab, transform);
            heart.sprite = fullHeartSprite;
            hearts.Add(heart);
        }
    }

    public void UpdateHearts(int currentHearts)
    {
        // Update the heart sprites based on current health
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHearts)
            {
                hearts[i].sprite = fullHeartSprite; // Set to full heart
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite; // Set to empty heart
            }
        }
    }
}
