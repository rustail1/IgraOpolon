using UnityEngine;
using Game;
public class FollowersHouse : MonoBehaviour
{
    public static FollowersHouse Instance;
    public OutpostTeam Team = OutpostTeam.Player;

    void Awake()
    {
        if(Team == OutpostTeam.Player) Instance = this;
    }
}
