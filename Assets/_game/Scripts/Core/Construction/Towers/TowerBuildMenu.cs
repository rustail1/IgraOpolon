using UnityEngine;
using Game;

public class TowerBuildMenu : MonoBehaviour
{
    public static TowerBuildMenu Instance;

    [Header("Available towers")]
    public GameObject[] Towers;

    [HideInInspector]
    public TowerBuildPoint BuildPoint;

    private RectTransform _rectTransform;
    private Camera _camera;

    private void Awake()
    {
        Instance = this;
        _rectTransform = GetComponent<RectTransform>();
        _camera = Camera.main;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (BuildPoint == null)
            return;

        if (_camera == null)
            _camera = Camera.main;

        _rectTransform.position =
            _camera.WorldToScreenPoint(BuildPoint.transform.position);
    }

    public void BuildTower(int index)
    {
        if (BuildPoint == null)
            return;

        if (BuildPoint.towerBuilt)
            return;

        if (index < 0 || index >= Towers.Length)
            return;

        GameObject prefab = Towers[index];

        if (prefab == null)
            return;

        Tower towerPrefab = prefab.GetComponentInChildren<Tower>();

        if (towerPrefab == null)
        {
            Debug.LogError($"Tower component not found in prefab {prefab.name}");
            return;
        }

        int price = Mathf.RoundToInt(towerPrefab.Price);

        if (CurrenciesModel.Instance == null)
        {
            Debug.LogError("CurrenciesModel.Instance == null");
            return;
        }

        if (!CurrenciesModel.Instance.TrySpend(CurrencyType.Gold, price))
        {
            Debug.Log("Недостаточно золота");
            return;
        }

        Tower tower = Instantiate(
            prefab,
            BuildPoint.transform.position,
            BuildPoint.transform.rotation,
            BuildPoint.transform)
            .GetComponentInChildren<Tower>();

        if (tower != null)
            tower.Team = OutpostTeam.Player;

        BuildPoint.towerBuilt = true;
        BuildPoint = null;

        gameObject.SetActive(false);
    }

    public void Close()
    {
        BuildPoint = null;
        gameObject.SetActive(false);
    }
}