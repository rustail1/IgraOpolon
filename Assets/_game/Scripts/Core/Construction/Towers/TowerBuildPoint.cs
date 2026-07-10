using UnityEngine;
using System.Collections;
using Game;

public class TowerBuildPoint : MonoBehaviour
{
    [HideInInspector] public bool towerBuilt = false;
    private void OnMouseDown()
    {
        if(GetOwner() == OutpostTeam.Player) StartCoroutine(BuildCoroutine());
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
    private OutpostTeam GetOwner()
    {
        try
        {
            return gameObject.GetComponentInParent<OutpostView>().Owner;
        }
        catch
        {
            return OutpostTeam.None;
        }
    }
}