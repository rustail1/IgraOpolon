using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{  
    [SerializeField] private AudioClip mainMusic;
    [HideInInspector] public static MusicPlayer Instance;
    [HideInInspector] public AudioSource AudioSource;
    private void Awake()
    {
        Instance = this;
        AudioSource = GetComponent<AudioSource>();
        AudioSource.loop = true;
        PlayMainMusic();
    }
    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || AudioSource.clip == clip && AudioSource.isPlaying) return;
        AudioSource.clip = clip;
        AudioSource.Play();
    }
    public void PlayMainMusic()
    {
        PlayMusic(mainMusic);
    }
}