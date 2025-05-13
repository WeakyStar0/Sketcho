using System;
using UnityEngine;

public class HealthItem : MonoBehaviour, iItem
{
    public int healthAmount = 1; // Amount of health to restore
    public static Action<int> OnHealthItemCollected;

    public void Collect()
    {
        OnHealthItemCollected?.Invoke(healthAmount);
        Destroy(gameObject);
    }
}
