using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneComponent : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void LoadScene()
    {
        Time.timeScale = 1f;
        MusicPlayer.Instance?.PlayMainMusic();
        SceneManager.LoadScene(sceneName);
    }
}