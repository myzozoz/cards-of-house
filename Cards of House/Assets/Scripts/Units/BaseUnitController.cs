using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnitController : MonoBehaviour, IUnit
{
    public TargetType ttype;
    public int team;
    public bool canAct = true;
    public IPlayer enemyPlayer;
    public int range;

    private IBoard board;
    private Grid grid;
    private System.Guid id;
    // Start is called before the first frame update
    void Awake()
    {
        id = System.Guid.NewGuid();
    }
    
    void Start()
    {
        board = transform.parent.GetComponent<IBoard>();
        grid = board.GetGrid();
        enemyPlayer = GetEnemyPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Execute()
    {
        //Choose target (enemy unit, enemy "player", powerup...)
        ITarget target = ChooseTarget(board.GetUnitsOnBoard());
        //Choose action strategy (walk, attack...)
        //Execute
        Debug.Log("Target found: " + target.GetLocation());
        //If target is in range, attack
        
        //otherwise walk

    }

    public TargetType GetTargetType()
    {
        return TargetType.Unit;
    }

    private IPlayer GetEnemyPlayer()
    {
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach (GameObject go in players) {
            IPlayer pc = go.GetComponent<IPlayer>();
            if (pc == null)
            {
                Debug.Log("No player controller found :(");
            }
            if (pc.GetTeam() != team)
            {
                return pc;
            }
        }
        return null;
    }

    private ITarget ChooseTarget(List<IUnit> units)
    {
        List<IUnit> filteredList = new List<IUnit>();
        foreach (IUnit unit in units) {
            if (unit.GetTeam() != team)
            {
                filteredList.Add(unit);
            }
        }
        if (filteredList.Count == 0)
        {
            return enemyPlayer;
        }
        return units[0];
    }

    public int GetTeam()
    {
        return team;
    }

    public Vector3Int GetLocation()
    {
        return grid.WorldToCell(transform.position);
    }

    public System.Guid GetId()
    {
        return id;
    }
}
