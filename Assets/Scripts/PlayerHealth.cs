using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public HealthUI healthUI;
    private SpriteRenderer spriteRenderer;

    [Header("Damage Cooldown")]
    [SerializeField] private float damageCooldown = 1f; // 1-second cooldown
    private bool canTakeDamage = true;

    public static event Action OnPlayerDeath;

    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EraserEnemyPatrol enemy = collision.GetComponent<EraserEnemyPatrol>();
        if (enemy != null && canTakeDamage)
        {
            TakeDamage(enemy.damage);
        }
    }

    private void TakeDamage(int damage)
    {
        if (!canTakeDamage) return; // Extra safety check

        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        StartCoroutine(FlashRed());
        StartCoroutine(DamageCooldown()); // Start cooldown

        if (currentHealth <= 0)
        {
            OnPlayerDeath.Invoke();
        }
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        float elapsed = 0;
        while (elapsed < damageCooldown)
        {
            // Flash every 0.1s during cooldown
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        spriteRenderer.enabled = true;
        canTakeDamage = true;
    }
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(1f);
        spriteRenderer.color = Color.white;
    }
}