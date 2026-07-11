using Game;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float Price;
    public string Name;
    public OutpostTeam Team;

    private void Update()
    {
        UpdateOwner();
    }
    private void UpdateOwner()
    {
        try
        {
            Team = gameObject.GetComponentInParent<OutpostView>().Owner;
        }
        catch
        {
            Team = OutpostTeam.Player;
        }
    }
}
