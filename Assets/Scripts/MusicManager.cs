using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public AudioSource audioSource;
    public AudioClip backgroundMusic;

    [Header("Volume Settings")]
    [SerializeField] private float normalVolume = 1f;
    [SerializeField] private float pausedVolume = 0.3f;
    [SerializeField] private float gameOverVolume = 0.2f;

    public static MusicManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = true;
                audioSource.spatialBlend = 0;
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (_instance != this) return;

        if (backgroundMusic != null && !audioSource.isPlaying)
        {
            PlayMusic(backgroundMusic);
        }

        GameController.OnPauseStateChanged += HandlePauseState;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource.clip == null && backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    public void PlayMusic(AudioClip clip = null)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
        }

        if (audioSource.clip != null)
        {
            audioSource.volume = normalVolume;
            audioSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    private void HandlePauseState(bool paused)
    {
        SetVolume(paused ? pausedVolume : normalVolume);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameController.OnPauseStateChanged -= HandlePauseState;
            _instance = null;
        }
    }
}