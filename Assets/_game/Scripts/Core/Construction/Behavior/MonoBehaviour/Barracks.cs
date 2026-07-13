using UnityEngine;
using Game;

public class Barracks : MonoBehaviour
{
    public static Barracks Instance;
    [HideInInspector] public OutpostTeam Team = OutpostTeam.Player;
    void Awake()
    {
        if (Team == OutpostTeam.Player) Instance = this;
    }
}
