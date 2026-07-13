using UnityEngine;
using TMPro;

public class GameResultWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ResultLabel;
    public static GameResultWindow Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    public void SetText(string text) => ResultLabel.SetText(text);
}
