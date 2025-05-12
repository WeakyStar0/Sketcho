using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // Maximum health of the player
    private int currentHealth; // Current health of the player
    public HealthUI healthUI; // Reference to the HealthUI script

    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for flashing effect
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth; // Initialize current health
        healthUI.SetMaxHearts(maxHealth); // Set the maximum hearts in the UI

        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EraserEnemyPatrol enemy = collision.GetComponent<EraserEnemyPatrol>();
        if (enemy != null)
        {
            TakeDamage(enemy.damage); // Call the TakeDamage method when colliding with an enemy
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce current health by damage
        healthUI.UpdateHearts(currentHealth); // Update the health UI

        // flash red cuz cool
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            // LE DED
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red; // Change color to red
        yield return new WaitForSeconds(0.2f); // Wait for a short duration
        spriteRenderer.color = Color.white; // Change color back to white
    }

}
