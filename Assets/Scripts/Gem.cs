using System;
using UnityEngine;

public class Gem : MonoBehaviour, iItem
{
    public static event Action<int> OnGemCollected;
    public int worth = 5;
    public void Collect()
    {
        OnGemCollected?.Invoke(worth);
        Destroy(gameObject);
    }
}
