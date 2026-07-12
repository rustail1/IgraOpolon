using Game;
using UnityEngine;

public class Forge : MonoBehaviour
{
    public static Forge Instance;
    [HideInInspector] public OutpostTeam Team = OutpostTeam.Player;
    void Awake()
    {
        if (Team == OutpostTeam.Player) Instance = this;
    }
}
