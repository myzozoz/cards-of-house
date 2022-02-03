using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IPlayer
{
    [SerializeField]
    private string aiName = "House";
    private int team = 1;

    private IBoard board;

    // Start is called before the first frame update
    void Start()
    {
        board = GameData.Instance.BoardObject.GetComponent<IBoard>();
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
        if (board.GetUnitsOnBoard(0).Count > 0)
            return TargetType.Unit;
        return TargetType.Avatar;
    }

    public Command GetCommand()
    {
        return Command.Move;
    }
}
