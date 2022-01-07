using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    public string playerName;
    public int team;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetName()
    {
        return playerName;
    }

    public int GetTeam()
    {
        return team;
    }

    public TargetType GetTargetType()
    {
        return TargetType.Player;
    }

    public Vector3Int GetLocation()
    {
        return new Vector3Int(0, 0, 0);
    }
}
