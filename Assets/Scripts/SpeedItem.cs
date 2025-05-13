using System;
using UnityEngine;

public class SpeedItem : MonoBehaviour, iItem
{
    public static event Action<float> OnSpeedItemCollected;
    public float speedMultiplier = 2f;
    public void Collect()
    {
        OnSpeedItemCollected?.Invoke(speedMultiplier);
        Destroy(gameObject);
    }
}
