using UnityEngine;
using Game;
public class GoldMine : MonoBehaviour
{
    public static GoldMine Instance;
    public OutpostTeam Team = OutpostTeam.Player;
    void Awake()
    {
        if (Team == OutpostTeam.Player) Instance = this;
    }
}
