using UnityEngine;
using System.Collections;

public class TowerBuildPoint : MonoBehaviour
{
    [HideInInspector] public bool towerBuilt = false;
    private void OnMouseDown()
    {
        StartCoroutine(BuildCoroutine());
    }
    private IEnumerator BuildCoroutine()
    {
        yield return new WaitForSeconds(0.15f);

        if (!towerBuilt)
        {
            TowerBuildMenu.Instance.BuildPoint = this;
            TowerBuildMenu.Instance.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            TowerBuildMenu.Instance.gameObject.SetActive(true);
        }          
    }
}