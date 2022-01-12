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

    public void Execute()
    {
        
        if (!canAct)
        {
            return;
        }
        Tilemap tm = board.GetTilemap();

        //Choose target (enemy unit, enemy "player", powerup...)

        ITarget target = ChooseTarget(board.GetTargetsOnBoard());
        Debug.Log($"Found target {target}");
        if (target == null)
        {
            Debug.Log("Target is null");
        }
        //Choose action strategy (walk, attack...)
        //Execute
        List<Vector3Int> possibleLocations = Navigator.FindLocationsWithinAttackDistance(tm, target, primaryReach, secondaryReach);
        Vector3Int closest = Navigator.FindNearestByWalkingDistance(tm, this.GetLocation(), possibleLocations, allowDiagonalMovement);
        //If target is in range, attack
        Debug.Log($"Current: {this.GetLocation()} Closest: {closest}");
        if (closest == this.GetLocation())
        {
            Attack(target);
        } else //otherwise walk
        {
            Approach(closest);
        }
    }

    private void Attack(ITarget target)
    {
        //Attack animation stuff here
        float damage = Random.Range(minDamage, maxDamage);
        Debug.Log($"Attacking for {damage} damage");
        target.TakeDamage(damage);
    }

    private void Approach(Vector3Int location)
    {
        Tilemap tm = board.GetTilemap();
        List<Vector3Int> steps = Navigator.FindPath(tm, this.GetLocation(), location, allowDiagonalMovement);
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
            if (pc.GetTeam() != team)
            {
                return pc;
            }
        }
        return null;
    }

    private ITarget ChooseTarget(List<ITarget> targets)
    {
        List<ITarget> filteredList = new List<ITarget>();

        //Remove friendlies from list
        foreach (ITarget target in targets) {
            if (target != null && target.GetTeam() != team)
            {
                filteredList.Add(target);
            }
        }
        
        //Prioritize units over players
        List<ITarget> sortedByTargetType = SortByTargetType(filteredList, new Dictionary<TargetType, float>() {
            { TargetType.Unit, 1f },
            { TargetType.Player, 2f },
            { TargetType.PowerUp, 0f },
        });


        //Prioritize by nearest in attack distance
        Debug.Log("Before attack distance sort");
        foreach (ITarget t in sortedByTargetType)
        {
            Debug.Log($"\t{t}");
        }
        List<ITarget> sortedByAttackDistance = SortByDistanceToAttack(sortedByTargetType);
        Debug.Log("After attack distance sort");
        foreach (ITarget t in sortedByAttackDistance)
        {
            Debug.Log($"\t{t}");
        }

        return sortedByAttackDistance[0];
    }

    private List<ITarget> SortByTargetType(List<ITarget> targets, Dictionary<TargetType, float> priorities)
    {
        List<ITarget> sorted = new List<ITarget>();
        foreach (ITarget t in targets)
        {
            float tPrio = priorities.ContainsKey(t.GetTargetType()) ? priorities[t.GetTargetType()] : 1f;
            int i = 0;
            while (i < sorted.Count && tPrio >= priorities[sorted[i].GetTargetType()])
            {
                i++;
            }
            sorted.Insert(i, t);
        }

        return sorted;
    }

    private List<ITarget> SortByDistanceToAttack(List<ITarget> targets)
    {
        List<ITarget> sorted = new List<ITarget>();
        Dictionary<ITarget, int> distances= new Dictionary<ITarget,int>();

        //Find target distances
        foreach (ITarget t in targets)
        {
            Debug.Log("Before finlasdfaf");
            List<Vector3Int> possibleLocations = Navigator.FindLocationsWithinAttackDistance(board.GetTilemap(), t, primaryReach, secondaryReach);
            Debug.Log("After finlasdfaf");
            Vector3Int closest = Navigator.FindNearestByWalkingDistance(board.GetTilemap(), this.GetLocation(), possibleLocations, allowDiagonalMovement);
            distances.Add(t, Navigator.FindPath(board.GetTilemap(), this.GetLocation(), closest, allowDiagonalMovement).Count);
        }
        
        //Sort by shortest distance
        foreach (ITarget t in targets)
        {
            int i = 0;
            while (i < sorted.Count && distances[t] >= distances[sorted[i]])
            {
                i++;
            }
            sorted.Insert(i, t);
        }

        return sorted;
    }

    private void MoveTo(Vector3Int target)
    {
        Vector3 targetPos = board.GetTilemap().GetCellCenterWorld(target) + new Vector3(0,heightOffset,0);
        transform.Translate(targetPos - transform.position);
    }

    public bool CanAct()
    {
        return canAct;
    }
}
