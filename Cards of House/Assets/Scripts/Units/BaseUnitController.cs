using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseUnitController : TargetController, IUnit
{
    public bool canAct = true;
    public int primaryReach;
    public int secondaryReach;
    public float heightOffset;
    public TileBase pathTile;   
    public float maxDamage;
    public float minDamage;
    public bool allowDiagonalMovement;
    public float moveTime = .5f;
    private TargetType targetMode = TargetType.Unit;


    new void Start()
    {
        base.Start();
        transform.position = new Vector3(transform.position.x,heightOffset, transform.position.z);
    }

    public void Execute(Command command)
    {
        
        if (!canAct)
        {
            return;
        }

        if (board == null)
        {
            board = GameData.Instance.BoardObject.GetComponent<IBoard>();
        }
        Tilemap tm = board.GetTilemap();

        //Choose target (enemy unit, enemy "player", powerup...)

        ITarget target = ChooseTarget(board.GetTargetsOnBoard());
        if (target == null)
        {
            Debug.Log("No target, releasing control");
            return;
        }
        //Choose action strategy (walk, attack...)
        //Execute
        //If target is in range, attack
        if (IsWithinAttackDistance(target))
        {
            Attack(target);
        }
        else //otherwise walk
        {
            List<Vector3Int> possibleLocations = Navigator.FindLocationsWithinAttackDistance(board, this.GetLocation(), target, primaryReach, secondaryReach, allowDiagonalMovement);
            Vector3Int closest = Navigator.FindNearestByWalkingDistance(board, this.GetLocation(), possibleLocations, allowDiagonalMovement);
            Approach(closest);
        }
    }

    private void Attack(ITarget target)
    {
        //Attack animation stuff here
        float damage = Random.Range(minDamage, maxDamage);
        //Debug.Log($"Attacking {target} for {damage} damage");
        target.TakeDamage(damage);
    }

    private void Approach(Vector3Int location)
    {
        //Debug.Log($"Approaching {location}");
        Tilemap tm = board.GetTilemap();
        List<Vector3Int> steps = Navigator.FindPath(board, this.GetLocation(), location, allowDiagonalMovement);
        board.ResetPathTiles();
        foreach (Vector3Int s in steps)
        {
            tm.SetTile(s, pathTile);
        }
        MoveTo(steps[steps.Count - 1]);
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
            if (pc.Team != team)
            {
                return pc;
            }
        }
        return null;
    }

    private ITarget ChooseTarget(List<ITarget> targets)
    {
        List<ITarget> filteredList = new List<ITarget>();
        //Debug.Log($"Choosing target from {targets.Count} options");
        //Remove friendlies from list
        foreach (ITarget target in targets) {
            if (target != null && target.Team != team)
            {
                filteredList.Add(target);
            }
        }
        //Debug.Log($"After filtering frendlies: {filteredList.Count}");
        
        //Prioritize units over players
        List<ITarget> filteredByTargetType = FilterByTargetType(filteredList, new List<TargetType>() {
            TargetType.Unit,
            TargetType.PowerUp,
            TargetType.Avatar,
        });
        //Debug.Log($"After filtering by target type: {filteredByTargetType.Count}");

        //Prioritize by nearest in attack distance
        List<ITarget> sortedByAttackDistance = SortByDistanceToAttack(filteredByTargetType);
        //Debug.Log($"After sorting by attack distance: {sortedByAttackDistance.Count}");

        return sortedByAttackDistance.Count > 0 ? sortedByAttackDistance[0] : null;
    }

    private List<ITarget> FilterByTargetType(List<ITarget> targets, List<TargetType> priorities)
    {
        Dictionary<TargetType, List<ITarget>> targetLists = new Dictionary<TargetType, List<ITarget>>();
        foreach (ITarget t in targets)
        {
            if (targetLists.ContainsKey(t.GetTargetType()))
            {
                targetLists[t.GetTargetType()].Add(t);
            }
            else
            {
                targetLists[t.GetTargetType()] = new List<ITarget>() { t };
            }
        }
        /*
        foreach (TargetType tt in priorities)
        {
            Debug.Log($"Found {targetLists[tt]} targets of type {tt}");
        }
        */
        foreach (TargetType tt in priorities)
        {
            if (targetLists.ContainsKey(tt) && targetLists[tt].Count > 0)
            {
                return targetLists[tt];
            }
        }

        return new List<ITarget>();
    }

    private List<ITarget> SortByDistanceToAttack(List<ITarget> targets)
    {
        List<ITarget> sorted = new List<ITarget>();
        Dictionary<ITarget, int> distances= new Dictionary<ITarget,int>();

        //Find target distances
        foreach (ITarget t in targets)
        {
            List<Vector3Int> possibleLocations = Navigator.FindLocationsWithinAttackDistance(board, this.GetLocation(), t, primaryReach, secondaryReach, allowDiagonalMovement);
            if (IsWithinAttackDistance(t))
            {
                possibleLocations.Add(t.GetLocation());
            }

            //Debug.Log("possible locations");
            //foreach (Vector3Int v in possibleLocations) { Debug.Log(v); }
            Vector3Int closest = Navigator.FindNearestByWalkingDistance(board, this.GetLocation(), possibleLocations, allowDiagonalMovement);
            distances.Add(t, closest == Navigator.ControlVector ? -1 : Navigator.FindPath(board, this.GetLocation(), closest, allowDiagonalMovement).Count);
        }

        //Sort by shortest distance
        foreach (ITarget t in targets)
        {
            int i = 0;
            while (i < sorted.Count && distances[t] >= distances[sorted[i]])
            {
                i++;
            }
            if (distances[t] >= 0)
            {
                sorted.Insert(i, t);
            }
        }

        //Debug.Log($"Sorted size: {sorted.Count}");
        return sorted;
    }

    private void MoveTo(Vector3Int target)
    {
        board.ReserveTile(target);
        board.ReleaseTile(GetLocation());
        Vector3 targetPos = board.GetTilemap().GetCellCenterWorld(target) + new Vector3(0,heightOffset,0);

        StartCoroutine(MoveRoutine(targetPos));

    }

    private IEnumerator MoveRoutine(Vector3 targetPos)
    {
        float timeSinceStart = 0f;
        Vector3 startPos = transform.position;
        while (timeSinceStart < moveTime)
        {
            timeSinceStart += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, timeSinceStart / moveTime);
            yield return null;
        }
    }

    public bool CanAct()
    {
        return canAct;
    }

    private bool IsWithinAttackDistance(ITarget target)
    {
        Vector3Int tl = target.GetLocation();
        Vector3Int l = this.GetLocation();
        Vector3Int dist = AbsVec(l - tl);
        if ((dist.x == 0 && dist.y <= primaryReach)
            || (dist.y == 0 && dist.x <= primaryReach)
            || (dist.x == dist.y && dist.x <= secondaryReach))
            return true;

        return false;
    }

    private Vector3Int AbsVec(Vector3Int v)
    {
        return new Vector3Int(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public TargetType TargetMode { 
        get { return targetMode; }
        set { targetMode = value; }
    }
}
