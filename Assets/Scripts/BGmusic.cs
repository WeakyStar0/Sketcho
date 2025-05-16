using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGmusic : MonoBehaviour
{
    public static BGmusic instance;

    [Header("Scene Music Clips")]
    public AudioClip mainMenuClip;
    public AudioClip level1Clip;
    public AudioClip level2Clip;
    public AudioClip creditsClip;

    private AudioSource audioSource;
    private string currentScene;
    private float defaultVolume = 1f;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        defaultVolume = audioSource.volume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        PlayMusicForScene(currentScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        PlayMusicForScene(currentScene);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip chosenClip = null;

        switch (sceneName)
        {
            case "MainMenu": chosenClip = mainMenuClip; break;
            case "Level1": chosenClip = level1Clip; break;
            case "Level2": chosenClip = level2Clip; break;
            case "Credits": chosenClip = creditsClip; break;
        }

        if (chosenClip != null && audioSource.clip != chosenClip)
        {
            audioSource.clip = chosenClip;
            audioSource.volume = defaultVolume;
            audioSource.Play();
        }
    }

    public void LowerVolume(float factor)
    {
        audioSource.volume = defaultVolume * factor;
    }

    public void RestoreVolume()
    {
        audioSource.volume = defaultVolume;
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void RestartMusic()
    {
        if (audioSource.clip != null)
        {
            audioSource.volume = defaultVolume;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
