using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseUnitController : MonoBehaviour, IUnit
{
    public TargetType ttype;
    public int team;
    public bool canAct = true;
    public IPlayer enemyPlayer;
    public int primaryReach;
    public int secondaryReach;
    public float heightOffset;
    public TileBase pathTile;

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
        if (!canAct)
        {
            return;
        }
        Tilemap tm = board.GetTilemap();

        //Choose target (enemy unit, enemy "player", powerup...)
        ITarget target = ChooseTarget(board.GetUnitsOnBoard());
        //Choose action strategy (walk, attack...)
        //Execute
        List<Vector3Int> possibleLocations = Navigator.FindLocationsWithinAttackDistance(tm, target, primaryReach, secondaryReach);
        Vector3Int closest = Navigator.FindNearestLocation(this.GetLocation(), possibleLocations);
        //If target is in range, attack
        if (closest == this.GetLocation())
        {
            Debug.Log("In range!");
        } else //otherwise walk
        {
            List<Vector3Int> steps = Navigator.FindNextStep(tm, this.GetLocation(), closest, false);
            board.ResetPathTiles();
            foreach (Vector3Int s in steps)
            {
                tm.SetTile(s, pathTile);
            }
            MoveTo(steps[steps.Count - 1]);
        }
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
        return filteredList[0];
    }

    public int GetTeam()
    {
        return team;
    }

    public Vector3Int GetLocation()
    {
        if (grid == null)
        {
            grid = board.GetGrid();
        }

        return grid.WorldToCell(transform.position);
    }

    public System.Guid GetId()
    {
        return id;
    }

    private void MoveTo(Vector3Int target)
    {
        Vector3 targetPos = board.GetTilemap().GetCellCenterWorld(target) + new Vector3(0,heightOffset,0);
        transform.Translate(targetPos - transform.position);
    }
}
