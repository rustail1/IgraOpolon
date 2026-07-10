using UnityEngine;
using TMPro;
using Game;

public class TowerBuildMenu : MonoBehaviour
{
    public static TowerBuildMenu Instance;
    public GameObject[] Towers;
    public TextMeshProUGUI[] Buttons;
    [HideInInspector]
    public TowerBuildPoint BuildPoint;


    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        
        if(Buttons.Length == Towers.Length)
        {
            for(int i = 0; i < Buttons.Length; i++)
            {
                var tower = Towers[i]?.GetComponentInChildren<Tower>();
                if (tower != null) Buttons[i]?.SetText($"{tower.Name} ({tower.Price})");
            }
        }
    }

    private void Update()
    {
        if (BuildPoint == null) return;
        transform.position = Camera.main.WorldToScreenPoint(BuildPoint.transform.position);
    }

    public void BuildTower(int index)
    {
        if (BuildPoint == null || BuildPoint.towerBuilt || index < 0 || index >= Towers.Length) return;
        GameObject prefab = Towers[index];

        if (prefab == null) return;

        Tower towerPrefab = prefab.GetComponentInChildren<Tower>();
        int price = Mathf.RoundToInt(towerPrefab.Price);

        if (!CurrenciesModel.Instance.TrySpend(CurrencyType.Gold, price)) return;

        Tower tower = Instantiate(prefab, BuildPoint.transform.position, BuildPoint.transform.rotation, BuildPoint.transform).GetComponentInChildren<Tower>();

        if (tower != null) tower.Team = OutpostTeam.Player;
            
        BuildPoint.towerBuilt = true;
        gameObject.SetActive(false);
    }

    public void Close()
    {
        BuildPoint = null;
        gameObject.SetActive(false);
    }
}