using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip mainMusic;

    public static MusicPlayer Instance { get; private set; }

    [HideInInspector]
    public AudioSource AudioSource;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource = GetComponent<AudioSource>();
        AudioSource.loop = true;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Если первая сцена — меню
        if (scene.buildIndex == 0)
        {
            PlayMenuMusic();
        }
        else
        {
            PlayMainMusic();
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (AudioSource.clip == clip && AudioSource.isPlaying)
            return;

        AudioSource.clip = clip;
        AudioSource.Play();
    }

    public void PlayMainMusic()
    {
        PlayMusic(mainMusic);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }
}