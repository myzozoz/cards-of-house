using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IPlayer
{
    [SerializeField]
    private string aiName = "House";
    private int team = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string Name
    {
        get { return aiName; }
    }

    public int Team
    {
        get { return team; }
    }

    public TargetType GetTarget()
    {
        return TargetType.Avatar;
    }

    public Command GetCommand()
    {
        return Command.Move;
    }
}
